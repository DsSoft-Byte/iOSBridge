const { app, BrowserWindow, ipcMain } = require('electron')
const path = require('path')
const fs   = require('fs')

function createWindow() {
  const win = new BrowserWindow({
    width: 520,
    height: 380,
    minWidth: 520,
    minHeight: 380,
    resizable: false,
    frame: false,
    transparent: false,
    backgroundColor: '#0a0a0c',
    webPreferences: {
      preload: path.join(__dirname, 'preload.js'),
      contextIsolation: true,
      nodeIntegration: false,
    }
  })
  win.loadFile('updater.html')
}

app.whenReady().then(createWindow)
app.on('window-all-closed', () => app.quit())

ipcMain.on('win-close', e => BrowserWindow.fromWebContents(e.sender).close())

// ── Copy update files and clean up ─────────────────────────────────────────
ipcMain.handle('apply-update', async () => {
  const src  = 'C:\\UpdateData\\iCures\\'
  const dest = 'C:\\iCures\\'

  if (!fs.existsSync(src)) throw new Error('Source directory not found: ' + src)

  copyDirSync(src, dest)

  // Write a VBScript to clean up C:\UpdateData after we exit
  const script = `On Error Resume Next\nDim objShell\nSet objShell = CreateObject("WScript.Shell")\nobjShell.Run "cmd /c rmdir /s /q C:\\UpdateData", 0, True\nSet objShell = Nothing`
  const scriptPath = require('os').tmpdir() + '\\DeleteUpdaterDirectory.vbs'
  fs.writeFileSync(scriptPath, script)

  // Run it detached so it fires after this process exits
  const { spawn } = require('child_process')
  spawn('cscript', [scriptPath], { detached: true, stdio: 'ignore' }).unref()

  return true
})

ipcMain.handle('quit-after-update', () => {
  app.quit()
})

function copyDirSync(src, dest) {
  fs.mkdirSync(dest, { recursive: true })
  for (const entry of fs.readdirSync(src, { withFileTypes: true })) {
    const s = path.join(src,  entry.name)
    const d = path.join(dest, entry.name)
    if (entry.isDirectory()) copyDirSync(s, d)
    else fs.copyFileSync(s, d)
  }
}
