const fs = require('fs')
const path = require('path')
const sharp = require('sharp')

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

// ICNS icon types and their pixel sizes
const ICNS_SIZES = [
  { size: 16,   type: 'icp4' },
  { size: 32,   type: 'icp5' },
  { size: 64,   type: 'icp6' },
  { size: 128,  type: 'ic07' },
  { size: 256,  type: 'ic08' },
  { size: 512,  type: 'ic09' },
  { size: 1024, type: 'ic10' },
]

async function main() {
  // Normalise to RGBA so sharp never composites against an implicit white background
  const src = await sharp(best.img).ensureAlpha().toBuffer()

  // Write icon.png (Linux build)
  const png = await sharp(src).png().toBuffer()
  fs.writeFileSync(PNG, png)
  console.log(`gen-icons: wrote icon.png (${best.w}×${best.w})`)

  // Build icon.icns (Mac build) — each size is a raw PNG chunk inside the ICNS container
  const chunks = []
  for (const { size, type } of ICNS_SIZES) {
    const data = await sharp(src)
      .resize(size, size, { kernel: sharp.kernel.lanczos3 })
      .png()
      .toBuffer()
    const hdr = Buffer.alloc(8)
    hdr.write(type)
    hdr.writeUInt32BE(8 + data.length, 4)
    chunks.push(hdr, data)
  }

  const body = Buffer.concat(chunks)
  const fileHdr = Buffer.alloc(8)
  fileHdr.write('icns')
  fileHdr.writeUInt32BE(8 + body.length, 4)
  fs.writeFileSync(ICNS, Buffer.concat([fileHdr, body]))
  console.log('gen-icons: wrote icon.icns')
}

main().catch(e => { console.error(e.message); process.exit(1) })
