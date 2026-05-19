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
  // When packaged, binaries are bundled inside the app's Resources/bin/
  if (app.isPackaged) return path.join(process.resourcesPath, 'bin') + path.sep
  if (MAC) {
    if (fs.existsSync('/opt/homebrew/bin/ideviceinfo')) return '/opt/homebrew/bin/'
    return '/usr/local/bin/'
  }
  return '/usr/bin/'
}

const BASE           = resolveBase()
const GASTER         = WIN ? 'C:\\iCures\\Dependencies\\gaster.exe' : BASE + 'gaster'
const IDEVICERESTORE = WIN
  ? 'C:\\iCures\\Dependencies\\libimdevice\\libimdevice\\idevicerestore.exe'
  : BASE + 'idevicerestore'

function createWindow(page, opts = {}) {
  const win = new BrowserWindow({
    width: opts.width || 1000,
    height: opts.height || 700,
    minWidth: 800,
    minHeight: 560,
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

app.whenReady().then(() => { createWindow('form13') })
app.on('window-all-closed', () => { if (!MAC) app.quit() })

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
ipcMain.handle('pwned-dfu', async () => {
  if (WIN) {
    runDetached(IDEVICERESTORE, ['--pwn'])
  } else if (MAC) {
    spawn('osascript', [
      '-e', 'tell application "Terminal" to activate',
      '-e', 'tell application "Terminal" to do script "/usr/local/bin/ipwndfu -p"',
    ])
  } else {
    const cmd = '/usr/local/bin/ipwndfu -p'
    const terms = [
      ['gnome-terminal',      ['--', 'bash', '-c', `${cmd}; read -p 'Done. Press Enter to close.'`]],
      ['xfce4-terminal',      ['-e', `bash -c "${cmd}; read"`]],
      ['konsole',             ['-e', `bash -c "${cmd}; read"`]],
      ['x-terminal-emulator', ['-e', `bash -c "${cmd}; read"`]],
      ['xterm',               ['-e', `bash -c "${cmd}; read"`]],
    ]
    let launched = false
    for (const [t, args] of terms) {
      try { execSync(`which ${t}`, { stdio: 'ignore' }); spawn(t, args, { detached: true }).unref(); launched = true; break } catch {}
    }
    if (!launched) spawn('bash', ['-c', cmd], { detached: true, stdio: 'ignore' }).unref()
  }
})

ipcMain.handle('custom-pwned-dfu', async () => {
  if (!WIN) { return }  // gaster is Windows-only
  runDetached(GASTER, ['pwn'])
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
ipcMain.handle('flash-ipsw', async (_, { filePath, useNewLib, erase }) => {
  if (WIN) {
    const exe       = BASE + (useNewLib ? 'idr1.exe' : 'idr.exe')
    const eraseFlag = erase ? '-e ' : ''
    const noInput   = erase ? '-y ' : ''
    const cmd = DEV_MODE
      ? `start "iCures Restore" cmd /k ""${exe}" ${noInput}${eraseFlag}"${filePath}""`
      : `start "iCures Restore" /wait "${exe}" ${noInput}${eraseFlag}"${filePath}"`
    exec(cmd, { shell: true })
  } else {
    const restore = IDEVICERESTORE
    const flags   = erase ? ['-e', filePath] : [filePath]
    if (MAC) {
      const cmdStr = `${restore} ${erase ? '-y -e ' : ''}"${filePath}"`
      spawn('osascript', [
        '-e', 'tell application "Terminal" to activate',
        '-e', `tell application "Terminal" to do script "${cmdStr.replace(/\\/g, '\\\\').replace(/"/g, '\\"')}"`,
      ])
    } else {
      // Linux: try common terminals in order
      const cmd = `${restore} ${erase ? '-y -e ' : ''}"${filePath}"`
      const terms = [
        ['gnome-terminal', ['--', 'bash', '-c', `${cmd}; read -p 'Done. Press Enter to close.'`]],
        ['xfce4-terminal', ['-e', `bash -c "${cmd}; read"`]],
        ['konsole',        ['-e', `bash -c "${cmd}; read"`]],
        ['x-terminal-emulator', ['-e', `bash -c "${cmd}; read"`]],
        ['xterm',          ['-e', `bash -c "${cmd}; read"`]],
      ]
      let launched = false
      for (const [t, args] of terms) {
        try { execSync(`which ${t}`, { stdio: 'ignore' }); spawn(t, args, { detached: true }).unref(); launched = true; break } catch {}
      }
      if (!launched) spawn('bash', ['-c', cmd], { detached: true, stdio: 'ignore' }).unref()
    }
  }
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
  }, 2000)
})

ipcMain.handle('stop-usb-watch', () => {
  clearInterval(usbPollInterval); usbPollInterval = null
})
