const { app, BrowserWindow, ipcMain, dialog, clipboard } = require('electron')
const { exec, spawn, execSync } = require('child_process')
const path = require('path')
const fs = require('fs')

// ── ⚠️  DEV MODE — set to false before shipping ───────────────────────────
const DEV_MODE = true

const BASE = 'C:\\iCures\\Dependencies\\lim\\'
const GASTER = 'C:\\iCures\\Dependencies\\gaster.exe'
const IDEVICERESTORE = 'C:\\iCures\\Dependencies\\libimdevice\\libimdevice\\idevicerestore.exe'

function createWindow(page, opts = {}) {
  const win = new BrowserWindow({
    width: opts.width || 1000,
    height: opts.height || 700,
    minWidth: 800,
    minHeight: 560,
    frame: false,
    transparent: true,
    backgroundColor: '#00000000',
    vibrancy: 'under-window',          // macOS glass (no-op on Win, handled by CSS)
    backgroundMaterial: 'acrylic',     // Windows 11 acrylic
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
})

app.on('window-all-closed', () => { if (process.platform !== 'darwin') app.quit() })

// ── Window chrome ──────────────────────────────────────────────────────────
ipcMain.on('win-minimize', e => BrowserWindow.fromWebContents(e.sender).minimize())
ipcMain.on('win-maximize', e => {
  const w = BrowserWindow.fromWebContents(e.sender)
  w.isMaximized() ? w.unmaximize() : w.maximize()
})
ipcMain.on('win-close', e => BrowserWindow.fromWebContents(e.sender).close())

// ── Open a sub-window ──────────────────────────────────────────────────────
ipcMain.handle('open-window', (_, page) => { createWindow(page) })

// ── Username file ──────────────────────────────────────────────────────────
ipcMain.handle('get-username', () => {
  const p = 'C:\\iCures\\Username.txt'
  return fs.existsSync(p) ? { found: true, name: fs.readFileSync(p, 'utf8').trim() } : { found: false }
})

// ── Generic helper: run exe, capture stdout ────────────────────────────────
function runCapture(exe, args = []) {
  return new Promise((resolve, reject) => {
    const p = spawn(exe, args, { shell: false })
    let out = '', err = ''
    p.stdout.on('data', d => { out += d })
    p.stderr.on('data', d => { err += d })
    p.on('close', code => code === 0 ? resolve(out.trim()) : reject(err.trim() || `Exit ${code}`))
  })
}

// ── Generic helper: fire-and-forget (visible console window) ──────────────
function runDetached(exe, args = []) {
  const p = spawn(exe, args, { detached: true, shell: false, stdio: 'ignore' })
  p.unref()
}

// ── Get UDID ───────────────────────────────────────────────────────────────
ipcMain.handle('get-udid', async () => {
  return runCapture(BASE + 'idevice_id.exe', ['-l'])
})

// ── Device info (on connect) ───────────────────────────────────────────────
ipcMain.handle('get-device-info', async () => {
  // run the bat, wait, read output.txt
  await new Promise(r => {
    exec('C:\\iCures\\ideviceinfopipe.bat', () => setTimeout(r, 4500))
  })
  try {
    return fs.readFileSync('C:\\iCures\\Dependencies\\lim\\output.txt', 'utf8')
  } catch { return 'Could not read device info.' }
})

// ── Enter recovery ─────────────────────────────────────────────────────────
ipcMain.handle('enter-recovery', async (_, udid) => {
  const args = udid ? [udid] : []
  runDetached(BASE + 'ideviceenterrecovery.exe', args)
})

// ── Exit recovery ──────────────────────────────────────────────────────────
ipcMain.handle('exit-recovery', async () => {
  runDetached(BASE + 'irecovery.exe', ['-n'])
})

// ── Pwned DFU (idevicerestore) ─────────────────────────────────────────────
ipcMain.handle('pwned-dfu', async () => {
  runDetached(IDEVICERESTORE, ['--pwn'])
})

// ── Custom Pwned DFU (gaster) ──────────────────────────────────────────────
ipcMain.handle('custom-pwned-dfu', async () => {
  runDetached(GASTER, ['pwn'])
})

// ── Restart device ─────────────────────────────────────────────────────────
ipcMain.handle('restart-device', async (_, udid) => {
  runDetached(BASE + 'idevicediagnostics.exe', ['restart', '-u', udid])
})

// ── Shutdown device ────────────────────────────────────────────────────────
ipcMain.handle('shutdown-device', async (_, udid) => {
  runDetached(BASE + 'idevicediagnostics.exe', ['shutdown', '-u', udid])
})

// ── Activate with specific server ──────────────────────────────────────────
ipcMain.handle('activate-with-server', async (_, server) => {
  runDetached(BASE + 'ideviceactivation.exe', ['activate', '-s', server, '-d'])
})

// ── Deactivate ─────────────────────────────────────────────────────────────
ipcMain.handle('deactivate', async () => {
  runDetached(BASE + 'ideviceactivation.exe', ['deactivate'])
})

// ── Machine ID (CPU ProcessorId + BIOS SerialNumber → SHA-256) ─────────────
ipcMain.handle('get-machine-id', async () => {
  try {
    const { execSync } = require('child_process')
    const cpu = execSync('wmic cpu get ProcessorId /value', { encoding: 'utf8' })
      .split('\n').find(l => l.startsWith('ProcessorId'))?.split('=')[1]?.trim() || 'Unknown'
    const bios = execSync('wmic bios get SerialNumber /value', { encoding: 'utf8' })
      .split('\n').find(l => l.startsWith('SerialNumber'))?.split('=')[1]?.trim() || 'Unknown'
    const combined = cpu + bios
    const crypto = require('crypto')
    return crypto.createHash('sha256').update(combined).digest('hex')
  } catch { return 'Unknown' }
})

// ── Open URL in default browser ────────────────────────────────────────────
const { shell } = require('electron')
ipcMain.handle('open-external', (_, url) => shell.openExternal(url))

// ── App version ────────────────────────────────────────────────────────────
ipcMain.handle('get-app-version', () => app.getVersion())

// ── OTA Update: check ──────────────────────────────────────────────────────
ipcMain.handle('check-for-update', async () => {
  const https = require('https')
  const http = require('http')

  function fetchFollowRedirects(url, maxRedirects = 5) {
    return new Promise((resolve, reject) => {
      const lib = url.startsWith('https') ? https : http
      lib.get(url, res => {
        if (res.statusCode >= 300 && res.statusCode < 400 && res.headers.location) {
          if (maxRedirects === 0) return reject(new Error('Too many redirects'))
          return fetchFollowRedirects(res.headers.location, maxRedirects - 1)
            .then(resolve).catch(reject)
        }
        if (res.statusCode !== 200) return reject(new Error(`HTTP ${res.statusCode}`))
        let data = ''
        res.on('data', d => { data += d })
        res.on('end', () => resolve(data.trim()))
      }).on('error', reject)
    })
  }

  const latest = await fetchFollowRedirects('https://dssoft.ch/ver.txt')
  const current = app.getVersion()
  return { current, latest, upToDate: latest.includes(current) }
})

// ── OTA Update: download + extract ────────────────────────────────────────
ipcMain.handle('download-update', async (e) => {
  const https = require('https')
  const AdmZip = require('adm-zip')   // npm install adm-zip
  const ZIP_PATH = 'C:\\iCuPlus.zip'
  const EXTRACT_PATH = 'C:\\UpdateData'

  if (fs.existsSync(ZIP_PATH)) {
    throw new Error('Update file already exists at C:\\iCuPlus.zip — please delete it first.')
  }

  // Download with progress
  await new Promise((resolve, reject) => {
    const file = fs.createWriteStream(ZIP_PATH)
    https.get('https://raw.githubusercontent.com/DsSoft-Byte/iCu-X/main/iCures.zip', res => {
      const total = parseInt(res.headers['content-length'] || '0', 10)
      let received = 0
      res.on('data', chunk => {
        received += chunk.length
        file.write(chunk)
        if (total > 0) {
          const pct = Math.round((received / total) * 100)
          BrowserWindow.fromWebContents(e.sender)?.webContents.send('update-progress', pct)
        }
      })
      res.on('end', () => { file.end(); resolve() })
      res.on('error', reject)
    }).on('error', reject)
  })

  // Extract
  const zip = new AdmZip(ZIP_PATH)
  zip.extractAllTo(EXTRACT_PATH, true)
})

// ── OTA Update: launch updater.exe and quit ────────────────────────────────
ipcMain.handle('launch-updater', () => {
  const UPDATER = 'C:\\UpdateData\\iCures\\icuplusupdater.exe'
  if (!fs.existsSync(UPDATER)) {
    throw new Error('Updater not found. Update data may be corrupted — check C:\\UpdateData.')
  }
  const { spawn } = require('child_process')
  spawn(UPDATER, [], { detached: true, stdio: 'ignore' }).unref()
  app.quit()
})
const ACTIVATION_FILE = path.join(app.getPath('userData'), 'activation.json')

ipcMain.handle('get-activated', () => {
  try {
    const data = JSON.parse(fs.readFileSync(ACTIVATION_FILE, 'utf8'))
    return data.activated === true
  } catch { return false }
})

ipcMain.handle('set-activated', (_, value) => {
  try {
    fs.mkdirSync(path.dirname(ACTIVATION_FILE), { recursive: true })
    fs.writeFileSync(ACTIVATION_FILE, JSON.stringify({ activated: value }))
  } catch { }
})

// ── Flash IPSW ─────────────────────────────────────────────────────────────
ipcMain.handle('flash-ipsw', async (_, { filePath, useNewLib, erase }) => {
  const exe = BASE + (useNewLib ? 'idr1.exe' : 'idr.exe')
  const eraseFlag = erase ? '-e ' : ''
  // -y auto-answers YES to the erase confirmation prompt
  const noInput = erase ? '-y ' : ''
  const { exec } = require('child_process')

  if (DEV_MODE) {
    // Keep window open with /k so you can read output while testing
    const cmd = `start "iCures Restore" cmd /k ""${exe}" ${noInput}${eraseFlag}"${filePath}""`
    exec(cmd, { shell: true })
  } else {
    // Production: window closes automatically when done
    const cmd = `start "iCures Restore" /wait "${exe}" ${noInput}${eraseFlag}"${filePath}"`
    exec(cmd, { shell: true })
  }
})

// ── Backup ─────────────────────────────────────────────────────────────────
ipcMain.handle('backup', async (_, { udid, dir, legacy }) => {
  const exe = legacy ? 'idevicebackup.exe' : 'idevicebackup2.exe'
  runDetached(BASE + exe, ['-u', udid, 'backup', dir])
})

// ── iProxy (TCP tunnel) ────────────────────────────────────────────────────
ipcMain.handle('iproxy', async (_, { udid, local, remote }) => {
  runDetached(BASE + 'iproxy.exe', ['-u', udid, local, remote])
})

// ── Copy to clipboard ──────────────────────────────────────────────────────
ipcMain.handle('copy-to-clipboard', (_, text) => { clipboard.writeText(text) })

// ── File picker (IPSW) ─────────────────────────────────────────────────────
ipcMain.handle('pick-ipsw', async (e) => {
  const win = BrowserWindow.fromWebContents(e.sender)
  const { canceled, filePaths } = await dialog.showOpenDialog(win, {
    title: 'Select IPSW',
    filters: [{ name: 'Apple IPSW', extensions: ['ipsw'] }],
    properties: ['openFile'],
  })
  return canceled ? null : filePaths[0]
})

// ── Folder picker (backup) ─────────────────────────────────────────────────
ipcMain.handle('pick-folder', async (e) => {
  const win = BrowserWindow.fromWebContents(e.sender)
  const { canceled, filePaths } = await dialog.showOpenDialog(win, {
    title: 'Select Backup Directory',
    properties: ['openDirectory'],
  })
  return canceled ? null : filePaths[0]
})

// ── USB device watcher (WMI via PowerShell, avoids heavy node dep) ─────────
// We poll via a renderer-side setInterval calling get-udid instead,
// but expose connect/disconnect events via a long-poll IPC channel.
let usbPollInterval = null
let lastUdid = ''

ipcMain.handle('start-usb-watch', async (e) => {
  if (usbPollInterval) return
  usbPollInterval = setInterval(async () => {
    try {
      const udid = await runCapture(BASE + 'idevice_id.exe', ['-l'])
      const win = BrowserWindow.fromWebContents(e.sender)
      if (!win || win.isDestroyed()) { clearInterval(usbPollInterval); return }
      if (udid && !lastUdid) {
        lastUdid = udid
        win.webContents.send('device-connected', udid)
      } else if (!udid && lastUdid) {
        lastUdid = ''
        win.webContents.send('device-disconnected')
      }
    } catch {
      if (lastUdid) { lastUdid = ''; BrowserWindow.fromWebContents(e.sender)?.webContents.send('device-disconnected') }
    }
  }, 2000)
})

ipcMain.handle('stop-usb-watch', () => {
  clearInterval(usbPollInterval)
  usbPollInterval = null
})