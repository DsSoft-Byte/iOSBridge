# iOSBridge

The complete iDevice toolkit. restore, activate, back up and repair iPhone & iPad on **macOS, Windows and Linux**. Built on [libimobiledevice](https://libimobiledevice.org); no iTunes required.

By [DsSoft](https://dssoft.ch).

## Features

- **Restore & flash IPSW** — signed firmware auto-selected, or point it at any IPSW (update or erase & restore)
- **Device activation** — iPad 2 & 4S, checkm8 devices, and A12+
- **Recovery & Pwned DFU** — enter/exit recovery, gaster/limera1n on Windows, ipwndfu on macOS/Linux
- **Full backups & restore** — any iOS version, any platform
- **SSH over USB** — iProxy port forwarding (default `2222→22`)
- **Live device report** — serial, IMEI, UDID, battery and storage

## Download

Grab the latest build for your platform from **[iosbridge.com](https://iosbridge.com/#download)** — `.dmg` (Apple Silicon / Intel), `.exe`, `.AppImage` or `.deb`.

### macOS: "App is damaged" error

Gatekeeper quarantines apps downloaded outside the App Store. Strip the flag once and it opens normally:

```sh
xattr -dr com.apple.quarantine /Applications/iOSBridge.app
```

## Activation

Some operations require an activation code. Get one at **[activation.iosbridge.com](https://activation.iosbridge.com)**.

## Links

- Website — <https://iosbridge.com>
- DsSoft — <https://dssoft.ch>
- Contact — [@the_hackintosh](https://t.me/the_hackintosh)

---

© 2026 DsSoft. All rights reserved.
