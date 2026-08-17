# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

iOSBridge is an Electron desktop app (Win/Mac/Linux) that wraps the `libimobiledevice` CLI toolchain to give a GUI for iDevice management: reading device info, entering/exiting recovery & DFU, flashing IPSW firmware, backup/restore, app install/uninstall, and app-license activation against a license server. There is a second, standalone Electron app in `icuupdater/` that only exists to apply OTA updates on Windows (it copies `C:\UpdateData` over `C:\iCures` and relaunches).

Not a git repository — there is no `.git` here, so don't assume `git log`/branches are available or try git operations.

## Commands

Run from the repo root unless noted.

```bash
npm start                 # launch the main app (electron .)
npm run build              # electron-builder, current platform
npm run build:mac          # electron-builder --mac
npm run build:win          # electron-builder --win
npm run build:linux        # electron-builder --linux
npm run build:deps         # scripts/build-deps.sh — builds libimobiledevice stack from source for mac/linux
npm run build:deps:clean   # same, but wipes staging + build/native first
```

`prebuild:mac` / `prebuild:linux` run automatically before `build:mac`/`build:linux` (electron-builder convention) and call `scripts/gen-icons.js` to regenerate `assets/icon.icns` from `assets/icon.png` using `sharp`.

There is no test runner, linter, or formatter configured in `package.json` — don't invent `npm test`/`npm run lint` commands.

`icuupdater/` is an independent Electron project (own `package.json`, own `node_modules`): `cd icuupdater && npm start` / `npm run build`.

## Architecture

**Two-process Electron model**, standard `contextIsolation: true` / `nodeIntegration: false`:

- `main.js` — the entire backend. All device/system access happens here via `ipcMain.handle`/`ipcMain.on`. No separate service layer or module split — everything (window management, iCures binary paths, IPSW download/flash, backup, activation, USB polling) lives in this one file.
- `preload.js` — the *only* bridge to the renderer. Exposes a single `window.app` object via `contextBridge`. Every IPC channel used by the UI must have a matching entry here; the renderer never talks to `ipcRenderer` directly.
- `pages/form13.html` — the actual UI. Single self-contained HTML file (inline `<style>` + inline `<script>`, no bundler/framework) that calls `app.*` methods from `preload.js`. This is the file to edit for UI/UX changes. `index.html` at the repo root is a separate, unused/legacy artifact — don't confuse it with `pages/form13.html`.
- `main.js` creates windows via `createWindow(page, opts)`, which loads `pages/${page}.html`; `app.openWindow(page)` (exposed in preload) is how the renderer opens additional windows.

**Native binary resolution** (`resolveBase()` in `main.js`): the app shells out to `libimobiledevice` CLI tools (`ideviceinfo`, `idevice_id`, `idevicediagnostics`, `idevicebackup2`, `idevicerestore`, `ideviceinstaller`, `irecovery`, `iproxy`, etc.) rather than linking a library. Resolution order:
1. Windows: always `C:\iCures\Dependencies\lim\`
2. Packaged app: bundled `resourcesPath/bin/` (from `build.*.extraResources` in `package.json`, populated by `scripts/build-deps.sh`)
3. macOS dev: `/opt/homebrew/bin/` or `/usr/local/bin/`
4. Fallback: `/usr/bin/`

Windows-only tools have hardcoded paths outside `BASE` (`GASTER` for `gaster.exe`, `IDEVICERESTORE` pointing at `libimdevice`). `EXT` appends `.exe` on Windows. When adding a new shell-out, follow this same `BASE + name + EXT` pattern and check platform branches (`WIN`/`MAC`) for tool availability — several features (e.g. `custom-pwned-dfu`/gaster, the DFU pwn terminal launcher) are platform-gated.

**`scripts/build-deps.sh`** (and its mac-ARM/Linux variants) clone and build the libimobiledevice stack from source (`libplist`, `libimobiledevice-glue`, `libusbmuxd`, `libtatsu`, `libimobiledevice`, `libideviceactivation`, `libirecovery`, `idevicerestore`, `ideviceinstaller`) into `build/native/{mac,linux}/{bin,libs}`, which electron-builder then bundles as `extraResources`. On Windows, dependencies are instead expected to already exist at `C:\iCures` on the target machine (installed via the NSIS custom install hook in `scripts/installer.nsh`, which robocopies bundled resources from the installer into `C:\iCures`).

**IPSW device identification**: `main.js` maintains an in-memory + disk-persisted cache (`ipsw-devices.json` in `userData`) of the ipsw.me `/v4/devices` list, refreshed on app startup when network is reachable (`refreshDeviceCache`) and read from disk otherwise (`loadDeviceCacheFromDisk`). `identify-recovery-device` matches a device in recovery/DFU by `cpid`/`bdid` against this cache; `check-signed-ipsw` and `download-ipsw` hit the live ipsw.me API directly for firmware listing/download. Any DFU/recovery-device-identification work should go through this cache rather than re-querying the network each time.

**Activation model**: license activation state is a boolean stored in `userData/activation.json` (`get-activated`/`set-activated`), keyed off a machine fingerprint (`get-machine-id`, SHA-256 of CPU/BIOS serial on Windows or `IOPlatformSerialNumber` on Mac). `DEV_MODE` at the top of `main.js` and mirrored in `pages/form13.html` bypasses activation checks entirely when `true` — it must be `false` before any shipping build (there's an inline warning comment marking this).

**OTA update flow**: `check-for-update`/`download-update` in `main.js` hit `https://dssoft.ch/ver.txt` and `https://www.iosbridge.com/iCures{,Mac,Lin}.zip`, extract to `C:\UpdateData` (Windows) or a temp dir (Mac/Linux), then hand off to the separate `icuupdater` app (`launch-updater`) to perform the actual file copy over the running install and relaunch — this two-app split exists because the main app can't overwrite its own running files.

## Conventions worth preserving

- No frontend build step: HTML/CSS/JS in `pages/*.html` is written and shipped as-is.
- Long-running/fire-and-forget native calls use `runDetached` (spawn + unref, no output captured); calls whose output the UI needs use `runCapture` or `runCaptureTimeout`; interactive/streaming calls (e.g. `flash-ipsw`) wire `stdout`/`stderr` to a `webContents.send` channel consumed by `onFlashOutput`-style listeners in preload.
- `.old` files (e.g. `package.json.old`, `scripts/build-deps.sh.old`) are kept as reference snapshots of a prior version — don't treat them as active config, and don't delete them without being asked.
