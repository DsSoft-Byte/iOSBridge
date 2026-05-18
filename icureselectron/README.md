# iCures Electron — Project Structure

```
icures-electron/
│
├── main.js              ← Electron main process (all Node/system calls live here)
├── preload.js           ← Secure IPC bridge (exposes window.app API to renderer)
├── package.json         ← npm / electron-builder config
│
├── pages/
│   ├── form13.html      ← iDevice Toolkit (main tool window)  ✅ done
│   ├── main.html        ← Launcher / home screen              (your Form5)
│   ├── form1.html       ← ...
│   ├── form3.html       ← ...
│   ├── form6.html       ← ...
│   ├── form8.html       ← ...
│   ├── form9.html       ← First-run / username setup
│   └── form10.html      ← ...
│
└── assets/
    ├── icon.ico
    └── (any images)
```

## Get started

```bash
npm install
npm start
```

## How IPC works (the mental model)

```
HTML button click
  → window.app.someAction()          (preload.js exposes this)
    → ipcRenderer.invoke('some-action')
      → ipcMain.handle('some-action')  (main.js handles this)
        → Node.js / child_process / fs
          → result returned to renderer
```

## Adding a new window (replacing a WinForms Form)

1. Create `pages/formN.html`
2. Call `app.openWindow('formN')` from any button
3. If it needs new system calls, add an `ipcMain.handle` in main.js
   and expose it via `contextBridge` in preload.js

## CSS design tokens (from your DsSoft site)

All CSS variables are defined at the top of each page's `<style>` block
and mirror your site's variables exactly:
  --bg-glass, --border-subtle, --glass-edge,
  --text-primary, --text-secondary, --accent, --accent-glow, etc.

Copy the :root block into any new page to stay on-brand.
