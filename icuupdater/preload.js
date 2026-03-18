const { contextBridge, ipcRenderer } = require('electron')

contextBridge.exposeInMainWorld('updater', {
  close:        ()  => ipcRenderer.send('win-close'),
  applyUpdate:  ()  => ipcRenderer.invoke('apply-update'),
  quit:         ()  => ipcRenderer.invoke('quit-after-update'),
})
