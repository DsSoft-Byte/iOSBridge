const fs = require('fs')
const path = require('path')
const sharp = require('sharp')

const PNG  = path.join(__dirname, '../assets/icon.png')
const ICNS = path.join(__dirname, '../assets/icon.icns')

if (!fs.existsSync(PNG)) {
  console.error('gen-icons: assets/icon.png not found.')
  process.exit(1)
}

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
  const src = await sharp(PNG).ensureAlpha().toBuffer()

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
  console.log('gen-icons: wrote icon.icns from icon.png')
}

main().catch(e => { console.error(e.message); process.exit(1) })
