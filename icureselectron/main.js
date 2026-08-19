const { app, BrowserWindow, ipcMain, dialog, clipboard, shell } = require('electron')
const { exec, spawn, execSync } = require('child_process')
const path = require('path')
const fs = require('fs')

// ── ⚠️  DEV MODE — set to false before shipping ───────────────────────────
const DEV_MODE = false

// ── Platform detection ────────────────────────────────────────────────────
const { platform } = process
const WIN   = platform === 'win32'
const MAC   = platform === 'darwin'
const EXT   = WIN ? '.exe' : ''

function resolveBase() {
  if (WIN) return 'C:\\iCures\\Dependencies\\lim\\'
  if (app.isPackaged) {
    const bundled = path.join(process.resourcesPath, 'bin') + path.sep
    if (fs.existsSync(bundled + 'ideviceinfo')) return bundled
  }
  if (MAC) {
    if (fs.existsSync('/opt/homebrew/bin/ideviceinfo')) return '/opt/homebrew/bin/'
    return '/usr/local/bin/'
  }
  return '/usr/bin/'
}

const BASE           = resolveBase()
const GASTER           = WIN ? 'C:\\iCures\\Dependencies\\gaster.exe' : BASE + 'gaster'
const IDEVICERESTORE   = BASE + 'idevicerestore' + EXT
const IDEVICEINSTALLER = BASE + 'ideviceinstaller' + EXT

function resolveSshrdDir() {
  if (app.isPackaged) return path.join(process.resourcesPath, 'sshrd')
  return path.join(__dirname, 'build', 'native', MAC ? 'mac' : 'linux', 'sshrd')
}
const SSHRD_DIR = WIN ? null : resolveSshrdDir()

// sshrd.sh writes into its own directory (mkdir logs, decompresses
// sshtars/*.tar.gz in place, creates work/ and sshramdisk/) — the bundled
// copy is read-only once packaged (AppImage's squashfs mount; root-owned
// /opt install prefixes on .deb/.rpm), so it's run from a writable copy
// under userData instead, copied once per app version.
let sshrdRuntimeDir = null
function resolveWritableSshrdDir() {
  if (!SSHRD_DIR) return null
  if (sshrdRuntimeDir) return sshrdRuntimeDir
  const dest = path.join(app.getPath('userData'), `sshrd-${app.getVersion()}`)
  if (!fs.existsSync(path.join(dest, 'sshrd.sh'))) {
    fs.rmSync(dest, { recursive: true, force: true })
    fs.mkdirSync(path.dirname(dest), { recursive: true })
    fs.cpSync(SSHRD_DIR, dest, { recursive: true })
    // cpSync preserves the bundled copy's mode bits — including read-only
    // directories on some packaging targets — so the whole tree needs write
    // permission restored before sshrd.sh can create logs/work/sshramdisk or
    // decompress sshtars/*.tar.gz in place. This is our own userData copy,
    // always owned by the current user, so no elevated permissions needed.
    execSync(`chmod -R u+w "${dest}"`)
  }
  sshrdRuntimeDir = dest
  return sshrdRuntimeDir
}

function createWindow(page, opts = {}) {
  const win = new BrowserWindow({
    width: opts.width || 1000,
    height: opts.height || 740,
    minWidth: 800,
    minHeight: 600,
    frame: false,
    transparent: true,
    backgroundColor: '#00000000',
    vibrancy: 'under-window',
    backgroundMaterial: 'acrylic',
    webPreferences: {
      preload: path.join(__dirname, 'preload.js'),
      contextIsolation: true,
      nodeIntegration: false,
    },
    ...opts,
  })
  win.loadFile(path.join(__dirname, 'pages', `${page}.html`))
  return win
}

app.whenReady().then(() => {
  createWindow('form13')
  // Warm the ipsw.me device table: load any disk cache, then refresh from network if reachable.
  loadDeviceCacheFromDisk()
  refreshDeviceCache().catch(() => {})
})
// No native menu bar or dock 'activate' reopen flow exists in this app, so
// the usual macOS convention of staying alive after all windows close would
// just strand it as a background zombie process — quit on all platforms.
app.on('window-all-closed', () => { app.quit() })
// Long-running child processes (gaster pwn, sshrd.sh) aren't killed by Electron
// on quit by default — without this they can be left running as orphans.
app.on('before-quit', () => {
  try { pwnChild && pwnChild.kill() } catch {}
  killSshrdGroup()
})

// ── Window chrome ──────────────────────────────────────────────────────────
ipcMain.on('win-minimize', e => BrowserWindow.fromWebContents(e.sender).minimize())
ipcMain.on('win-maximize', e => {
  const w = BrowserWindow.fromWebContents(e.sender)
  w.isMaximized() ? w.unmaximize() : w.maximize()
})
ipcMain.on('win-close', e => BrowserWindow.fromWebContents(e.sender).close())
ipcMain.handle('open-window', (_, page) => { createWindow(page) })

// ── Username file ──────────────────────────────────────────────────────────
ipcMain.handle('get-username', () => {
  const p = 'C:\\iCures\\Username.txt'
  return fs.existsSync(p) ? { found: true, name: fs.readFileSync(p, 'utf8').trim() } : { found: false }
})

// ── Generic helpers ────────────────────────────────────────────────────────
function runCapture(exe, args = []) {
  return new Promise((resolve, reject) => {
    const p = spawn(exe, args, { shell: false })
    let out = '', err = ''
    p.stdout.on('data', d => { out += d })
    p.stderr.on('data', d => { err += d })
    p.on('close', code => code === 0 ? resolve(out.trim()) : reject(err.trim() || `Exit ${code}`))
  })
}

function runCaptureTimeout(exe, args = [], ms = 2500) {
  return new Promise((resolve, reject) => {
    const p = spawn(exe, args, { shell: false })
    let out = '', err = ''
    const t = setTimeout(() => { try { p.kill() } catch {} ; reject(new Error('timeout')) }, ms)
    p.stdout.on('data', d => { out += d })
    p.stderr.on('data', d => { err += d })
    p.on('close', code => { clearTimeout(t); code === 0 ? resolve(out.trim()) : reject(err.trim() || `Exit ${code}`) })
  })
}

function runDetached(exe, args = []) {
  const p = spawn(exe, args, { detached: true, shell: false, stdio: 'ignore' })
  p.unref()
}

// Windows-only: pops a real, visible cmd.exe window running exe/args and
// leaves it open (/k) so the user can actually see limera1n/gaster output —
// there's no in-app streaming equivalent on Windows (unlike mac/linux).
// windowsVerbatimArguments is required here: without it Node re-quotes each
// argv entry as if it were a normal program argument, which mangles the
// composite `start "title" cmd /k <command>` line that cmd.exe itself
// re-parses with its own quoting rules — so the command line must be built
// and quoted by hand instead.
// cwd is set to the exe's own directory: `start` otherwise inherits whatever
// directory Electron's main process happened to be launched from, and both
// idevicerestore.exe and gaster.exe expect their sibling DLLs to be
// resolvable from the current directory.
function spawnVisibleTerminal(title, exe, args) {
  const quote = s => /[\s"]/.test(s) ? `"${String(s).replace(/"/g, '""')}"` : s
  const cmdLine = [exe, ...args].map(quote).join(' ')
  spawn('cmd.exe', ['/c', 'start', `"${title}"`, 'cmd.exe', '/k', cmdLine], {
    cwd: path.dirname(exe),
    detached: true, shell: false, windowsHide: false, windowsVerbatimArguments: true, stdio: 'ignore',
  }).unref()
}

// ── Get UDID ───────────────────────────────────────────────────────────────
ipcMain.handle('get-udid', async () => {
  return runCapture(BASE + 'idevice_id' + EXT, ['-l'])
})

// ── Full device info dump ──────────────────────────────────────────────────
ipcMain.handle('get-device-info', async () => {
  try {
    return await runCapture(BASE + 'ideviceinfo' + EXT, [])
  } catch { return 'Could not read device info.' }
})

// ── Single device field via -k ─────────────────────────────────────────────
ipcMain.handle('get-device-field', async (_, key) => {
  try {
    return await runCapture(BASE + 'ideviceinfo' + EXT, ['-k', key])
  } catch { return '' }
})

// ── Battery info via diagnostics ───────────────────────────────────────────
ipcMain.handle('get-battery-info', async () => {
  const tryEntry = async (entry) => {
    try { return await runCapture(BASE + 'idevicediagnostics' + EXT, ['ioregentry', entry]) }
    catch { return '' }
  }
  const modern = await tryEntry('AppleSmartBattery')
  if (modern.includes('CurrentCapacity')) return modern
  return await tryEntry('AppleARMPMUCharger')
})

// ── Recovery / DFU device query ───────────────────────────────────────────────
ipcMain.handle('check-recovery-device', async () => {
  try {
    return await runCaptureTimeout(BASE + 'irecovery' + EXT, ['-q'], 2500)
  } catch { return '' }
})

// ── Disk usage domain ──────────────────────────────────────────────────────
ipcMain.handle('get-disk-usage', async () => {
  try {
    return await runCapture(BASE + 'ideviceinfo' + EXT, ['-q', 'com.apple.disk_usage'])
  } catch { return '' }
})

// ── Enter recovery ─────────────────────────────────────────────────────────
ipcMain.handle('enter-recovery', async (_, udid) => {
  runDetached(BASE + 'ideviceenterrecovery' + EXT, udid ? [udid] : [])
})

// ── Exit recovery ──────────────────────────────────────────────────────────
ipcMain.handle('exit-recovery', async () => {
  runDetached(BASE + 'irecovery' + EXT, ['-n'])
})

// ── Pwned DFU ──────────────────────────────────────────────────────────────
// Windows uses idevicerestore's built-in limera1n pwn. macOS/Linux stream a
// bundled `gaster pwn` run into the renderer, same pattern as flash-ipsw.
// checkm8 can legitimately hang mid-exploit on some chips/revisions (no
// automatic timeout upstream), so the child is tracked for pwn-dfu-cancel.
let pwnChild = null

ipcMain.handle('pwned-dfu', async (e) => {
  if (WIN) {
    spawnVisibleTerminal('iOSBridge - Pwned DFU (limera1n)', IDEVICERESTORE, ['--pwn'])
    return
  }
  if (pwnChild) return { code: -1, error: 'A Pwned DFU attempt is already running' }
  const send = (type, text) => {
    const win = BrowserWindow.fromWebContents(e.sender)
    if (win && !win.isDestroyed()) win.webContents.send('pwn-output', { type, text })
  }
  return new Promise(resolve => {
    const p = spawn(GASTER, ['pwn'], { shell: false })
    pwnChild = p
    p.stdout.on('data', d => send('out', d.toString()))
    p.stderr.on('data', d => send('out', d.toString()))
    p.on('close', code => { pwnChild = null; send('done', code); resolve({ code }) })
    p.on('error', err  => { pwnChild = null; send('err', err.message); resolve({ code: -1 }) })
  })
})

ipcMain.handle('pwn-dfu-cancel', () => {
  if (pwnChild) { try { pwnChild.kill() } catch {} ; pwnChild = null }
})

ipcMain.handle('custom-pwned-dfu', async () => {
  if (!WIN) { return }  // gaster is Windows-only
  spawnVisibleTerminal('iOSBridge - Pwned DFU (gaster)', GASTER, ['pwn'])
})

// ── Restart / Shutdown ─────────────────────────────────────────────────────
ipcMain.handle('restart-device',  async (_, udid) => { runDetached(BASE + 'idevicediagnostics' + EXT, ['restart',  '-u', udid]) })
ipcMain.handle('shutdown-device', async (_, udid) => { runDetached(BASE + 'idevicediagnostics' + EXT, ['shutdown', '-u', udid]) })

// ── Activation ─────────────────────────────────────────────────────────────
ipcMain.handle('activate-with-server', async (_, server) => {
  runDetached(BASE + 'ideviceactivation' + EXT, ['activate', '-s', server, '-d'])
})
ipcMain.handle('deactivate', async () => {
  runDetached(BASE + 'ideviceactivation' + EXT, ['deactivate'])
})

// ── Machine ID ─────────────────────────────────────────────────────────────
ipcMain.handle('get-machine-id', async () => {
  try {
    const crypto = require('crypto')
    let combined
    if (WIN) {
      const cpu  = execSync('wmic cpu get ProcessorId /value', { encoding: 'utf8' })
        .split('\n').find(l => l.startsWith('ProcessorId'))?.split('=')[1]?.trim() || 'Unknown'
      const bios = execSync('wmic bios get SerialNumber /value', { encoding: 'utf8' })
        .split('\n').find(l => l.startsWith('SerialNumber'))?.split('=')[1]?.trim() || 'Unknown'
      combined = cpu + bios
    } else if (MAC) {
      const serial = execSync('ioreg -l | grep IOPlatformSerialNumber', { encoding: 'utf8' })
        .match(/"IOPlatformSerialNumber" = "(.+?)"/)?.[1] || 'Unknown'
      combined = serial + platform
    } else {
      const mid = fs.existsSync('/etc/machine-id')
        ? fs.readFileSync('/etc/machine-id', 'utf8').trim()
        : 'Unknown'
      combined = mid + platform
    }
    return crypto.createHash('sha256').update(combined).digest('hex')
  } catch { return 'Unknown' }
})

// ── Open URL ───────────────────────────────────────────────────────────────
ipcMain.handle('open-external', (_, url) => shell.openExternal(url))

// ── App version ────────────────────────────────────────────────────────────
ipcMain.handle('get-app-version', () => app.getVersion())

// ── OTA: check ────────────────────────────────────────────────────────────
ipcMain.handle('check-for-update', async () => {
  const https = require('https')
  const http  = require('http')
  function fetchText(url, hops = 5) {
    return new Promise((resolve, reject) => {
      const lib = url.startsWith('https') ? https : http
      lib.get(url, res => {
        if (res.statusCode >= 300 && res.statusCode < 400 && res.headers.location) {
          return hops > 0 ? fetchText(res.headers.location, hops - 1).then(resolve).catch(reject) : reject(new Error('Too many redirects'))
        }
        if (res.statusCode !== 200) return reject(new Error(`HTTP ${res.statusCode}`))
        let d = ''; res.on('data', c => { d += c }); res.on('end', () => resolve(d.trim()))
      }).on('error', reject)
    })
  }
  const latest  = await fetchText('https://dssoft.ch/ver.txt')
  const current = app.getVersion()
  return { current, latest, upToDate: latest.includes(current) }
})

// ── OTA: download ─────────────────────────────────────────────────────────
ipcMain.handle('download-update', async (e) => {
  const https    = require('https')
  const AdmZip   = require('adm-zip')
  const ZIP_PATH     = WIN ? 'C:\\iCuPlus.zip' : path.join(app.getPath('temp'), 'iCuPlus.zip')
  const EXTRACT_PATH = WIN ? 'C:\\UpdateData'  : path.join(app.getPath('temp'), 'iCuresUpdate')
  if (fs.existsSync(ZIP_PATH)) throw new Error(`Update file already exists at ${ZIP_PATH} — please delete it first.`)
  await new Promise((resolve, reject) => {
    const file = fs.createWriteStream(ZIP_PATH)
    const ZIP_URL = WIN ? 'https://www.iosbridge.com/iCures.zip'
                 : MAC ? 'https://www.iosbridge.com/iCuresMac.zip'
                 :       'https://www.iosbridge.com/iCuresLin.zip'
    https.get(ZIP_URL, res => {
      const total = parseInt(res.headers['content-length'] || '0', 10)
      let received = 0
      res.on('data', chunk => {
        received += chunk.length; file.write(chunk)
        if (total > 0) BrowserWindow.fromWebContents(e.sender)?.webContents.send('update-progress', Math.round((received / total) * 100))
      })
      res.on('end', () => { file.end(); resolve() }); res.on('error', reject)
    }).on('error', reject)
  })
  const zip = new AdmZip(ZIP_PATH)
  zip.extractAllTo(EXTRACT_PATH, true)
})

// ── OTA: launch updater ────────────────────────────────────────────────────
ipcMain.handle('launch-updater', () => {
  const UPDATER = WIN
    ? 'C:\\UpdateData\\iCures\\icuplusupdater.exe'
    : path.join(app.getPath('temp'), 'iCuresUpdate', 'iCures', 'icuplusupdater')
  if (!fs.existsSync(UPDATER)) throw new Error('Updater not found.')
  spawn(UPDATER, [], { detached: true, stdio: 'ignore' }).unref()
  app.quit()
})

// ── App activation storage ─────────────────────────────────────────────────
const ACTIVATION_FILE = path.join(app.getPath('userData'), 'activation.json')
ipcMain.handle('get-activated', () => {
  try { return JSON.parse(fs.readFileSync(ACTIVATION_FILE, 'utf8')).activated === true } catch { return false }
})
ipcMain.handle('set-activated', (_, value) => {
  try { fs.mkdirSync(path.dirname(ACTIVATION_FILE), { recursive: true }); fs.writeFileSync(ACTIVATION_FILE, JSON.stringify({ activated: value })) } catch {}
})

// ── Flash IPSW ─────────────────────────────────────────────────────────────
ipcMain.handle('flash-ipsw', async (e, { filePath, useNewLib, erase }) => {
  const send = (type, text) => {
    const win = BrowserWindow.fromWebContents(e.sender)
    if (win && !win.isDestroyed()) win.webContents.send('flash-output', { type, text })
  }
  let exe, args
  if (WIN) {
    exe  = BASE + (useNewLib ? 'idr1.exe' : 'idr.exe')
    args = [...(erase ? ['-y', '-e'] : []), filePath]
  } else {
    exe  = IDEVICERESTORE
    args = [...(erase ? ['-y', '-e'] : []), filePath]
  }
  return new Promise(resolve => {
    const p = spawn(exe, args, { shell: false, windowsHide: true })
    p.stdout.on('data', d => send('out', d.toString()))
    p.stderr.on('data', d => send('out', d.toString()))
    p.on('close', code => { send('done', code); resolve({ code }) })
    p.on('error', err  => { send('err', err.message); resolve({ code: -1 }) })
  })
})

// ── IPSW: check signed firmware via ipsw.me API ─────────────────────────────
ipcMain.handle('check-signed-ipsw', async (_, identifier) => {
  if (!identifier) return []
  const https = require('https')
  const raw = await new Promise((resolve, reject) => {
    https.get(`https://api.ipsw.me/v4/device/${encodeURIComponent(identifier)}?type=ipsw`,
      { headers: { 'User-Agent': 'iOSBridge', 'Accept': 'application/json' } }, res => {
        if (res.statusCode !== 200) return reject(new Error(`HTTP ${res.statusCode}`))
        let d = ''; res.on('data', c => { d += c }); res.on('end', () => resolve(d))
      }).on('error', reject)
  })
  const json = JSON.parse(raw)
  return (json.firmwares || [])
    .filter(f => f.signed)
    .map(f => ({ version: f.version, buildid: f.buildid, url: f.url, filesize: f.filesize, identifier: f.identifier }))
})

// ── IPSW: device table (ipsw.me) with persistent, self-refreshing cache ─────
// Full /v4/devices list is cached to disk so recovery/DFU identifier lookup
// works offline. Whenever the network is reachable we re-fetch and re-cache,
// so newly launched devices are picked up automatically on the next online run.
const IPSW_DEVICES_CACHE = path.join(app.getPath('userData'), 'ipsw-devices.json')
let _ipswDeviceCache = null

function loadDeviceCacheFromDisk() {
  if (_ipswDeviceCache) return _ipswDeviceCache
  try {
    const arr = JSON.parse(fs.readFileSync(IPSW_DEVICES_CACHE, 'utf8'))
    if (Array.isArray(arr) && arr.length) _ipswDeviceCache = arr
  } catch {}
  return _ipswDeviceCache
}

async function refreshDeviceCache() {
  const https = require('https')
  const raw = await new Promise((resolve, reject) => {
    https.get('https://api.ipsw.me/v4/devices',
      { headers: { 'User-Agent': 'iOSBridge', 'Accept': 'application/json' } }, res => {
        if (res.statusCode !== 200) return reject(new Error(`HTTP ${res.statusCode}`))
        let d = ''; res.on('data', c => { d += c }); res.on('end', () => resolve(d))
      }).on('error', reject)
  })
  const arr = JSON.parse(raw)
  if (Array.isArray(arr) && arr.length) {
    _ipswDeviceCache = arr
    try { fs.mkdirSync(path.dirname(IPSW_DEVICES_CACHE), { recursive: true }); fs.writeFileSync(IPSW_DEVICES_CACHE, raw) } catch {}
  }
  return _ipswDeviceCache
}

// Identifier → friendly name map (e.g. iPhone16,2 → "iPhone 15 Pro Max"),
// used as a fallback for models not in the renderer's hardcoded table.
ipcMain.handle('get-device-names', async () => {
  let list = _ipswDeviceCache || loadDeviceCacheFromDisk()
  if (!list) { try { list = await refreshDeviceCache() } catch {} }
  const map = {}
  if (list) for (const dev of list) if (dev.identifier && dev.name) map[dev.identifier] = dev.name
  return map
})

ipcMain.handle('identify-recovery-device', async (_, { cpid, bdid }) => {
  if (!cpid || bdid == null) return null
  const cpidNum = parseInt(cpid, 16)
  const bdidNum = parseInt(bdid, 16)
  if (isNaN(cpidNum) || isNaN(bdidNum)) return null
  // Use the warm cache (memory or disk); only hit the network if we have nothing.
  // The startup refresh keeps the cache current, so this stays fast and offline-safe.
  let list = _ipswDeviceCache || loadDeviceCacheFromDisk()
  if (!list) { try { list = await refreshDeviceCache() } catch {} }
  if (!list) return null
  const match = list.find(dev => {
    const boards = (dev.boards && dev.boards.length) ? dev.boards : [{ cpid: dev.cpid, bdid: dev.bdid }]
    return boards.some(b => b.cpid === cpidNum && b.bdid === bdidNum)
  })
  return match ? match.identifier : null
})

// ── IPSW: download signed firmware (streams progress) ───────────────────────
ipcMain.handle('download-ipsw', async (e, { url, identifier, version, buildid }) => {
  const https = require('https')
  const dir = app.getPath('downloads')
  const fileName = `${identifier}_${version}_${buildid}.ipsw`.replace(/[^\w.\-]/g, '_')
  const finalPath = path.join(dir, fileName)
  if (fs.existsSync(finalPath) && fs.statSync(finalPath).size > 0) return { filePath: finalPath, reused: true }

  const partPath = finalPath + '.part'
  await new Promise((resolve, reject) => {
    const get = (u, hops = 5) => {
      https.get(u, { headers: { 'User-Agent': 'iOSBridge' } }, res => {
        if (res.statusCode >= 300 && res.statusCode < 400 && res.headers.location) {
          res.resume()
          return hops > 0 ? get(res.headers.location, hops - 1) : reject(new Error('Too many redirects'))
        }
        if (res.statusCode !== 200) return reject(new Error(`HTTP ${res.statusCode}`))
        const total = parseInt(res.headers['content-length'] || '0', 10)
        let received = 0
        const file = fs.createWriteStream(partPath)
        res.on('data', chunk => {
          received += chunk.length
          const win = BrowserWindow.fromWebContents(e.sender)
          if (win && !win.isDestroyed())
            win.webContents.send('ipsw-download-progress', { received, total, pct: total ? Math.round(received / total * 100) : 0 })
        })
        res.pipe(file)
        file.on('finish', () => file.close(() => resolve()))
        file.on('error', err => { try { fs.unlinkSync(partPath) } catch {} reject(err) })
        res.on('error', reject)
      }).on('error', reject)
    }
    get(url)
  })
  fs.renameSync(partPath, finalPath)
  return { filePath: finalPath, reused: false }
})

// ── Backup ─────────────────────────────────────────────────────────────────
ipcMain.handle('backup', async (_, { udid, dir }) => {
  runDetached(BASE + 'idevicebackup2' + EXT, ['-u', udid, 'backup', dir])
})

ipcMain.handle('restore-backup', async (_, { udid, dir }) => {
  runDetached(BASE + 'idevicebackup2' + EXT, ['-u', udid, 'restore', dir])
})

ipcMain.handle('pair-device', async () => {
  try {
    return await runCapture(BASE + 'idevicepair' + EXT, ['pair'])
  } catch (e) { return e.toString() }
})

// ── iProxy ─────────────────────────────────────────────────────────────────
ipcMain.handle('iproxy', async (_, { udid, local, remote }) => {
  runDetached(BASE + 'iproxy' + EXT, ['-u', udid, local, remote])
})

// iproxy tunnels are launched detached/unref'd with no PID tracking (each
// one is fire-and-forget), so "kill all" targets the process by name instead
// of trying to keep a live handle list.
ipcMain.handle('kill-all-iproxy', async () => {
  return new Promise(resolve => {
    const p = WIN
      ? spawn('taskkill', ['/F', '/IM', 'iproxy.exe', '/T'], { shell: false })
      : spawn('pkill', ['-f', 'iproxy'], { shell: false })
    p.on('close', code => resolve({ code }))
    p.on('error', () => resolve({ code: -1 }))
  })
})

// ── Clipboard ──────────────────────────────────────────────────────────────
ipcMain.handle('copy-to-clipboard', (_, text) => { clipboard.writeText(text) })

// ── File pickers ───────────────────────────────────────────────────────────
ipcMain.handle('pick-ipsw', async (e) => {
  const { canceled, filePaths } = await dialog.showOpenDialog(BrowserWindow.fromWebContents(e.sender), {
    title: 'Select IPSW', filters: [{ name: 'Apple IPSW', extensions: ['ipsw'] }], properties: ['openFile'],
  })
  return canceled ? null : filePaths[0]
})
ipcMain.handle('pick-folder', async (e) => {
  const { canceled, filePaths } = await dialog.showOpenDialog(BrowserWindow.fromWebContents(e.sender), {
    title: 'Select Backup Directory', properties: ['openDirectory'],
  })
  return canceled ? null : filePaths[0]
})
ipcMain.handle('pick-ipa', async (e) => {
  const { canceled, filePaths } = await dialog.showOpenDialog(BrowserWindow.fromWebContents(e.sender), {
    title: 'Select IPA', filters: [{ name: 'iOS App', extensions: ['ipa'] }], properties: ['openFile'],
  })
  return canceled ? null : filePaths[0]
})

// ── App management ─────────────────────────────────────────────────────────
ipcMain.handle('list-apps', async () => {
  const out = await runCapture(IDEVICEINSTALLER, ['list', '--user'])
  return out.split('\n').slice(1).map(line => {
    const m = line.match(/^([^,]+),\s*"([^"]*)",\s*"(.*)"/)
    return m ? { bundleId: m[1].trim(), version: m[2], name: m[3] } : null
  }).filter(Boolean)
})

ipcMain.handle('install-app', async (_, filePath) => {
  return runCapture(IDEVICEINSTALLER, ['install', filePath])
})

ipcMain.handle('uninstall-app', async (_, bundleId) => {
  return runCapture(IDEVICEINSTALLER, ['uninstall', bundleId])
})

// ── USB watcher ────────────────────────────────────────────────────────────
let usbPollInterval = null
let lastUdid = ''

ipcMain.handle('start-usb-watch', async (e) => {
  if (usbPollInterval) return
  usbPollInterval = setInterval(async () => {
    try {
      const udid = await runCapture(BASE + 'idevice_id' + EXT, ['-l'])
      const win = BrowserWindow.fromWebContents(e.sender)
      if (!win || win.isDestroyed()) { clearInterval(usbPollInterval); usbPollInterval = null; return }
      if (udid && !lastUdid) { lastUdid = udid; win.webContents.send('device-connected', udid) }
      else if (!udid && lastUdid) { lastUdid = ''; win.webContents.send('device-disconnected') }
    } catch {
      if (lastUdid) { lastUdid = ''; BrowserWindow.fromWebContents(e.sender)?.webContents.send('device-disconnected') }
    }
  }, 750)
})

ipcMain.handle('stop-usb-watch', () => {
  clearInterval(usbPollInterval); usbPollInterval = null
})

// ── SSHRD (SSH Ramdisks) ────────────────────────────────────────────────────
// Wraps the bundled SSHRD_Script (sshrd.sh) — a self-contained shell script
// with its own prebuilt tools — invoked from its own directory since it uses
// paths relative to itself (sshtars/, other/, work/, "$oscheck"/<tool>).
let sshrdChild = null

function sshrdSend(e, type, text) {
  const win = BrowserWindow.fromWebContents(e.sender)
  if (win && !win.isDestroyed()) win.webContents.send('sshrd-output', { type, text })
}

// sshrd.sh itself shells out to gaster/irecovery/pzb/img4 etc — killing just
// the `sh` PID leaves those grandchildren running as orphans. Spawning
// detached makes `sh` the leader of its own process group, so killing the
// whole group (negative PID) reaches every descendant it started.
function killSshrdGroup() {
  if (!sshrdChild) return
  try { process.kill(-sshrdChild.pid, 'SIGTERM') } catch {}
  sshrdChild = null
}

// Covers all one-shot subcommands: ramdisk creation (args: [iosVersion]),
// 'boot', 'reboot', 'reset', 'dump-blobs', 'clean'.
ipcMain.handle('sshrd-run', async (e, args = []) => {
  if (WIN || !SSHRD_DIR) return { code: -1, error: 'Not supported on this platform' }
  if (sshrdChild) return { code: -1, error: 'An SSHRD operation is already running' }
  let cwd
  try { cwd = resolveWritableSshrdDir() } catch (err) { return { code: -1, error: 'Could not prepare SSHRD tools: ' + err.message } }
  return new Promise(resolve => {
    const p = spawn('sh', ['sshrd.sh', ...args], { cwd, shell: false, detached: true })
    sshrdChild = p
    p.stdout.on('data', d => sshrdSend(e, 'out', d.toString()))
    p.stderr.on('data', d => sshrdSend(e, 'out', d.toString()))
    p.on('close', code => { sshrdChild = null; sshrdSend(e, 'done', code); resolve({ code }) })
    p.on('error', err  => { sshrdChild = null; sshrdSend(e, 'err', err.message); resolve({ code: -1 }) })
  })
})

// 'ssh' is the one interactive case — keeps stdin open for sshrd-send-input.
ipcMain.handle('sshrd-ssh', async (e) => {
  if (WIN || !SSHRD_DIR) return { code: -1, error: 'Not supported on this platform' }
  if (sshrdChild) return { code: -1, error: 'An SSHRD operation is already running' }
  let cwd
  try { cwd = resolveWritableSshrdDir() } catch (err) { return { code: -1, error: 'Could not prepare SSHRD tools: ' + err.message } }
  return new Promise(resolve => {
    const p = spawn('sh', ['sshrd.sh', 'ssh'], { cwd, shell: false, detached: true })
    sshrdChild = p
    p.stdout.on('data', d => sshrdSend(e, 'out', d.toString()))
    p.stderr.on('data', d => sshrdSend(e, 'out', d.toString()))
    p.on('close', code => { sshrdChild = null; sshrdSend(e, 'done', code); resolve({ code }) })
    p.on('error', err  => { sshrdChild = null; sshrdSend(e, 'err', err.message); resolve({ code: -1 }) })
  })
})

ipcMain.handle('sshrd-send-input', (_, text) => {
  if (sshrdChild && sshrdChild.stdin && !sshrdChild.stdin.destroyed) sshrdChild.stdin.write(text + '\n')
})

ipcMain.handle('sshrd-disconnect', () => { killSshrdGroup() })
