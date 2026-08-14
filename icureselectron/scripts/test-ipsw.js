// Standalone test of the ipsw.me integration logic (mirrors main.js exactly).
// Verifies: signed-IPSW check, device-table cache + offline fallback,
// recovery CPID/BDID resolution, and the identifier→name map.
const https = require('https')
const fs = require('fs')
const path = require('path')
const os = require('os')

const CACHE = path.join(os.tmpdir(), 'ipsw-devices-test.json')
let cache = null

function getJson(url) {
  return new Promise((resolve, reject) => {
    https.get(url, { headers: { 'User-Agent': 'iOSBridge', 'Accept': 'application/json' } }, res => {
      if (res.statusCode !== 200) return reject(new Error(`HTTP ${res.statusCode}`))
      let d = ''; res.on('data', c => { d += c }); res.on('end', () => resolve(d))
    }).on('error', reject)
  })
}

function loadDeviceCacheFromDisk() {
  if (cache) return cache
  try { const a = JSON.parse(fs.readFileSync(CACHE, 'utf8')); if (Array.isArray(a) && a.length) cache = a } catch {}
  return cache
}
async function refreshDeviceCache(url = 'https://api.ipsw.me/v4/devices') {
  const raw = await getJson(url)
  const a = JSON.parse(raw)
  if (Array.isArray(a) && a.length) { cache = a; fs.writeFileSync(CACHE, raw) }
  return cache
}
function resolve(list, cpid, bdid) {
  const c = parseInt(cpid, 16), b = parseInt(bdid, 16)
  const m = list.find(dev => {
    const boards = (dev.boards && dev.boards.length) ? dev.boards : [{ cpid: dev.cpid, bdid: dev.bdid }]
    return boards.some(x => x.cpid === c && x.bdid === b)
  })
  return m ? m.identifier : null
}

let pass = 0, fail = 0
function check(name, cond, extra = '') { (cond ? (pass++, console.log(`  PASS  ${name} ${extra}`)) : (fail++, console.log(`  FAIL  ${name} ${extra}`))) }

;(async () => {
  try { fs.unlinkSync(CACHE) } catch {}

  console.log('1) check-signed-ipsw (iPhone16,2)')
  const fw = JSON.parse(await getJson('https://api.ipsw.me/v4/device/iPhone16,2?type=ipsw'))
  const signed = (fw.firmwares || []).filter(f => f.signed).map(f => ({ version: f.version, buildid: f.buildid, url: f.url, filesize: f.filesize, identifier: f.identifier }))
  check('at least one signed firmware', signed.length >= 1, `(${signed.length})`)
  check('signed entry has url + filesize', !!signed[0]?.url && signed[0]?.filesize > 0, signed[0] ? `${signed[0].version}/${signed[0].buildid}` : '')

  console.log('2) refresh device cache -> disk')
  await refreshDeviceCache()
  check('cache populated', Array.isArray(cache) && cache.length > 100, `(${cache.length})`)
  check('cache written to disk', fs.existsSync(CACHE))

  console.log('3) recovery CPID/BDID resolution')
  check('0x8130/0x06 -> iPhone16,2', resolve(cache, '0x8130', '0x06') === 'iPhone16,2', resolve(cache, '0x8130', '0x06'))
  check('0x8010/0x08 -> iPhone9,1', resolve(cache, '0x8010', '0x08') === 'iPhone9,1', resolve(cache, '0x8010', '0x08'))
  check('0x8015/0x06 -> iPhone10,3 (A11 disambiguated by bdid)', resolve(cache, '0x8015', '0x06') === 'iPhone10,3', resolve(cache, '0x8015', '0x06'))
  check('0x8150/0x0e -> iPhone18,2 (iPhone 17 Pro Max, new chip)', resolve(cache, '0x8150', '0x0e') === 'iPhone18,2', resolve(cache, '0x8150', '0x0e'))
  check('unknown cpid/bdid -> null', resolve(cache, '0x9999', '0x99') === null)

  console.log('4) OFFLINE fallback (simulate no network)')
  cache = null                                   // clear memory
  const disk = loadDeviceCacheFromDisk()         // must reload from disk
  check('reloaded from disk cache', Array.isArray(disk) && disk.length > 100, `(${disk?.length})`)
  check('offline resolve still works', resolve(disk, '0x8130', '0x06') === 'iPhone16,2')
  cache = null
  let offlineList
  try { offlineList = await refreshDeviceCache('https://api.ipsw.me/v4/DOES_NOT_EXIST_offline') } catch { offlineList = loadDeviceCacheFromDisk() }
  check('network fail falls back to disk', Array.isArray(offlineList) && offlineList.length > 100)

  console.log('5) get-device-names map')
  const map = {}
  for (const dev of disk) if (dev.identifier && dev.name) map[dev.identifier] = dev.name
  check('map has iPhone16,2 name', /iPhone 15 Pro Max/i.test(map['iPhone16,2'] || ''), map['iPhone16,2'])
  check('map covers many devices', Object.keys(map).length > 100, `(${Object.keys(map).length})`)

  try { fs.unlinkSync(CACHE) } catch {}
  console.log(`\nRESULT: ${pass} passed, ${fail} failed`)
  process.exit(fail ? 1 : 0)
})().catch(e => { console.error('TEST ERROR:', e.message); process.exit(2) })
