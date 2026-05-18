const fs = require('fs')
const path = require('path')
const png2icons = require('png2icons')

const ICO  = path.join(__dirname, '../assets/icon.ico')
const PNG  = path.join(__dirname, '../assets/icon.png')
const ICNS = path.join(__dirname, '../assets/icon.icns')

// Parse ICO and extract the largest PNG-compressed entry
const buf   = fs.readFileSync(ICO)
const count = buf.readUInt16LE(4)

let best = null
for (let i = 0; i < count; i++) {
  const e   = 6 + i * 16
  const w   = buf[e] || 256
  const len = buf.readUInt32LE(e + 8)
  const off = buf.readUInt32LE(e + 12)
  const img = buf.slice(off, off + len)
  const isPng = img[0] === 0x89 && img[1] === 0x50 && img[2] === 0x4e && img[3] === 0x47
  if (isPng && (!best || w > best.w)) best = { w, img }
}

if (!best) {
  console.error('gen-icons: no PNG-compressed entry found in icon.ico.')
  console.error('Re-export the ICO with a 256×256 PNG-compressed entry included.')
  process.exit(1)
}

fs.writeFileSync(PNG, best.img)
console.log(`gen-icons: extracted ${best.w}×${best.w} PNG from icon.ico`)

const icns = png2icons.createICNS(best.img, png2icons.BILINEAR, 0)
if (!icns) { console.error('gen-icons: ICNS conversion failed.'); process.exit(1) }
fs.writeFileSync(ICNS, icns)
console.log('gen-icons: created icon.icns')
