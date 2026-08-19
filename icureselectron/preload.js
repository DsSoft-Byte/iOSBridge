const { contextBridge, ipcRenderer } = require('electron')

contextBridge.exposeInMainWorld('app', {
  // Window chrome
  minimize:   () => ipcRenderer.send('win-minimize'),
  maximize:   () => ipcRenderer.send('win-maximize'),
  close:      () => ipcRenderer.send('win-close'),

  // Navigation
  openWindow: (page)   => ipcRenderer.invoke('open-window', page),

  // Identity
  getUsername: ()      => ipcRenderer.invoke('get-username'),

  // Device
  getUdid:        ()       => ipcRenderer.invoke('get-udid'),
  getDeviceInfo:  ()       => ipcRenderer.invoke('get-device-info'),
  getDeviceField: (key)    => ipcRenderer.invoke('get-device-field', key),
  getBatteryInfo: ()       => ipcRenderer.invoke('get-battery-info'),
  getDiskUsage:        ()       => ipcRenderer.invoke('get-disk-usage'),
  checkRecoveryDevice: ()       => ipcRenderer.invoke('check-recovery-device'),
  startUsbWatch:       ()       => ipcRenderer.invoke('start-usb-watch'),
  stopUsbWatch:   ()       => ipcRenderer.invoke('stop-usb-watch'),

  // Device control
  enterRecovery:    (udid)            => ipcRenderer.invoke('enter-recovery', udid),
  exitRecovery:     ()                => ipcRenderer.invoke('exit-recovery'),
  pwnedDfu:         ()                => ipcRenderer.invoke('pwned-dfu'),
  customPwnedDfu:   ()                => ipcRenderer.invoke('custom-pwned-dfu'),
  restartDevice:    (udid)            => ipcRenderer.invoke('restart-device', udid),
  shutdownDevice:   (udid)            => ipcRenderer.invoke('shutdown-device', udid),
  activateWithServer: (server)         => ipcRenderer.invoke('activate-with-server', server),
  deactivate:         ()               => ipcRenderer.invoke('deactivate'),

  // App activation
  getMachineId:  ()      => ipcRenderer.invoke('get-machine-id'),
  getActivated:  ()      => ipcRenderer.invoke('get-activated'),
  setActivated:  (val)   => ipcRenderer.invoke('set-activated', val),
  flashIpsw:        (opts)            => ipcRenderer.invoke('flash-ipsw', opts),
  onFlashOutput:    (cb) => { ipcRenderer.removeAllListeners('flash-output'); ipcRenderer.on('flash-output', (_, d) => cb(d)) },
  onPwnOutput:      (cb) => { ipcRenderer.removeAllListeners('pwn-output'); ipcRenderer.on('pwn-output', (_, d) => cb(d)) },
  pwnDfuCancel:     ()                => ipcRenderer.invoke('pwn-dfu-cancel'),
  backup:           (opts)            => ipcRenderer.invoke('backup', opts),
  restoreBackup:    (opts)            => ipcRenderer.invoke('restore-backup', opts),
  pairDevice:       ()                => ipcRenderer.invoke('pair-device'),
  iproxy:           (opts)            => ipcRenderer.invoke('iproxy', opts),
  killAllIproxy:    ()                => ipcRenderer.invoke('kill-all-iproxy'),

  // SSHRD (SSH Ramdisks)
  sshrdRun:         (args)  => ipcRenderer.invoke('sshrd-run', args),
  sshrdSsh:         ()      => ipcRenderer.invoke('sshrd-ssh'),
  sshrdSendInput:   (text)  => ipcRenderer.invoke('sshrd-send-input', text),
  sshrdDisconnect:  ()      => ipcRenderer.invoke('sshrd-disconnect'),
  onSshrdOutput:    (cb) => { ipcRenderer.removeAllListeners('sshrd-output'); ipcRenderer.on('sshrd-output', (_, d) => cb(d)) },

  // IPSW auto-download (ipsw.me)
  checkSignedIpsw: (identifier)       => ipcRenderer.invoke('check-signed-ipsw', identifier),
  getDeviceNames:  ()                 => ipcRenderer.invoke('get-device-names'),
  identifyRecoveryDevice: (opts)      => ipcRenderer.invoke('identify-recovery-device', opts),
  downloadIpsw:    (opts)             => ipcRenderer.invoke('download-ipsw', opts),
  onIpswProgress:  (cb) => { ipcRenderer.removeAllListeners('ipsw-download-progress'); ipcRenderer.on('ipsw-download-progress', (_, d) => cb(d)) },

  // File pickers
  pickIpsw:   ()       => ipcRenderer.invoke('pick-ipsw'),
  pickFolder: ()       => ipcRenderer.invoke('pick-folder'),
  pickIpa:    ()       => ipcRenderer.invoke('pick-ipa'),

  // App management
  listApps:     ()           => ipcRenderer.invoke('list-apps'),
  installApp:   (filePath)   => ipcRenderer.invoke('install-app', filePath),
  uninstallApp: (bundleId)   => ipcRenderer.invoke('uninstall-app', bundleId),

  // Clipboard
  copy: (text)         => ipcRenderer.invoke('copy-to-clipboard', text),

  // Browser / external links
  openExternal: (url)  => ipcRenderer.invoke('open-external', url),

  // Platform
  isWin: process.platform === 'win32',

  // App version
  getAppVersion: ()    => ipcRenderer.invoke('get-app-version'),

  // OTA updates
  checkForUpdate: () => ipcRenderer.invoke('check-for-update'),

  // USB events from main
  onDeviceConnected:    (cb) => ipcRenderer.on('device-connected',    (_, udid) => cb(udid)),
  onDeviceDisconnected: (cb) => ipcRenderer.on('device-disconnected', ()        => cb()),
})
