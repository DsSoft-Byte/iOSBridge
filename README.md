# iOSBridge

The complete iDevice toolkit. restore, activate, back up and repair iPhone & iPad on **macOS, Windows and Linux**. Built on [libimobiledevice](https://libimobiledevice.org). No iTunes required.

By [DsSoft](https://dssoft.ch).

## Features

- **Restore & flash IPSW**      Signed firmware auto-selected, or point it at any IPSW (update or erase & restore)
- **Device activation**      iPad 2 & 4S, checkm8 devices, and A12+
- **Recovery & Pwned DFU**      Enter/exit recovery, gaster/limera1n on Windows, gaster on macOS/Linux
- **SSH Ramdisks**      Create and boot SSH Ramdisks on any alloc8 or checkm8 device
- **Full backups & restore**      Any iOS version, any platform
- **SSH over USB**      iProxy port forwarding (default `2222→22`)
- **Live device report**      Serial, IMEI, UDID, battery and storage

## Upcoming

- **BridgeBox Support**      Custom DsSoft engineered hardware for pwning checkm8-a5 and usbliter8 (A12-A13) devices

## Download

Grab the latest build for your platform from **[iosbridge.com](https://iosbridge.com/#download)** `.dmg` (Apple Silicon / Intel), `.exe`, `.AppImage` or `.deb`.

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
