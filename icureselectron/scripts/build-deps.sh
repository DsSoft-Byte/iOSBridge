#!/usr/bin/env bash
# scripts/build-deps.sh
#
# Clones and builds the full libimobiledevice stack from source, then bundles
# the resulting binaries + shared libraries into build/native/{mac,linux}/ for
# use as Electron extraResources.
#
# macOS prerequisites (your existing brew script covers all of these):
#   Xcode CLI Tools
#   brew install automake libtool libusb libzip gnutls libgcrypt pkg-config libxml2 curl dylibbundler
#
# Linux prerequisites:
#   sudo apt install build-essential git automake libtool pkg-config patchelf \
#     libusb-1.0-0-dev libgnutls28-dev libzip-dev libcurl4-openssl-dev libxml2-dev
#
# Usage:
#   bash scripts/build-deps.sh           # build and bundle
#   bash scripts/build-deps.sh --clean   # wipe staging + output then rebuild

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(dirname "$SCRIPT_DIR")"
STAGING="$REPO_ROOT/build/deps-staging"

# ── Platform detection ─────────────────────────────────────────────────────
case "$OSTYPE" in
  darwin*) PLATFORM=mac  ;;
  linux*)  PLATFORM=linux ;;
  *) echo "Unsupported platform: $OSTYPE"; exit 1 ;;
esac

OUT_BIN="$REPO_ROOT/build/native/$PLATFORM/bin"
OUT_LIBS="$REPO_ROOT/build/native/$PLATFORM/libs"

if [[ "${1:-}" == "--clean" ]]; then
  echo "[clean] Wiping $STAGING and $REPO_ROOT/build/native/$PLATFORM"
  rm -rf "$STAGING" "$REPO_ROOT/build/native/$PLATFORM"
fi

mkdir -p "$STAGING" "$OUT_BIN" "$OUT_LIBS"
CPUS=$(nproc 2>/dev/null || sysctl -n hw.ncpu 2>/dev/null || echo 4)

# ── Build environment ──────────────────────────────────────────────────────
if [[ "$PLATFORM" == mac ]]; then
  BREW_PREFIX="$(brew --prefix)"
  export PKG_CONFIG_PATH="\
$STAGING/lib/pkgconfig:\
$BREW_PREFIX/lib/pkgconfig:\
$BREW_PREFIX/opt/libxml2/lib/pkgconfig:\
$BREW_PREFIX/opt/openssl@3/lib/pkgconfig:\
$BREW_PREFIX/opt/curl/lib/pkgconfig\
${PKG_CONFIG_PATH:+:$PKG_CONFIG_PATH}"
  export LDFLAGS="-L$STAGING/lib -L$BREW_PREFIX/lib -L$BREW_PREFIX/opt/libxml2/lib${LDFLAGS:+ $LDFLAGS}"
  export CPPFLAGS="-I$STAGING/include -I$BREW_PREFIX/include -I$BREW_PREFIX/opt/libxml2/include${CPPFLAGS:+ $CPPFLAGS}"
else
  export PKG_CONFIG_PATH="$STAGING/lib/pkgconfig${PKG_CONFIG_PATH:+:$PKG_CONFIG_PATH}"
  export LDFLAGS="-L$STAGING/lib${LDFLAGS:+ $LDFLAGS}"
  export CPPFLAGS="-I$STAGING/include${CPPFLAGS:+ $CPPFLAGS}"
fi
export LD_LIBRARY_PATH="$STAGING/lib${LD_LIBRARY_PATH:+:$LD_LIBRARY_PATH}"

# ── Clone → build → install into $STAGING ─────────────────────────────────
BUILD_DIR="$(mktemp -d)"
trap 'rm -rf "$BUILD_DIR"' EXIT
cd "$BUILD_DIR"

build_repo() {
  local url="$1"; shift
  local name="${url##*/}"; name="${name%.git}"
  echo ""
  echo "━━━ $name ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
  git clone --depth=1 "$url" "$name"
  pushd "$name" > /dev/null
  ./autogen.sh "$@" --prefix="$STAGING"
  make -j"$CPUS"
  make install
  popd > /dev/null
}

build_repo "https://github.com/libimobiledevice/libplist"              --without-cython
build_repo "https://github.com/libimobiledevice/libimobiledevice-glue"
build_repo "https://github.com/libimobiledevice/libusbmuxd"
build_repo "https://github.com/libimobiledevice/libimobiledevice"      --without-cython --disable-openssl
build_repo "https://github.com/OliTheRepairDude/libideviceactivation"
build_repo "https://github.com/libimobiledevice/libirecovery"
build_repo "https://github.com/libimobiledevice/idevicerestore"

# ── Collect binaries ───────────────────────────────────────────────────────
echo ""
echo "━━━ Collecting binaries ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
BINARIES=(
  ideviceinfo idevice_id idevicediagnostics ideviceenterrecovery
  idevicebackup2 idevicepair iproxy irecovery ideviceactivation idevicerestore
)
for b in "${BINARIES[@]}"; do
  if [[ -f "$STAGING/bin/$b" ]]; then
    cp -L "$STAGING/bin/$b" "$OUT_BIN/$b"
    chmod +x "$OUT_BIN/$b"
    echo "  ✓ $b"
  else
    echo "  ✗ MISSING: $b — check build output above"
  fi
done

# ══════════════════════════════════════════════════════════════════════════════
# macOS: bundle dylibs with dylibbundler
# dylibbundler handles install-name canonicalisation so the same library never
# appears under two different identities (e.g. libplist.4.dylib vs
# libplist-2.0.4.dylib), which is what caused the duplicate-dylib errors.
# ══════════════════════════════════════════════════════════════════════════════
if [[ "$PLATFORM" == mac ]]; then

  command -v dylibbundler > /dev/null || {
    echo "dylibbundler not found — installing via brew..."
    brew install dylibbundler
  }

  echo ""
  echo "━━━ Bundling dylibs (dylibbundler) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
  for b in "${BINARIES[@]}"; do
    [[ -f "$OUT_BIN/$b" ]] || continue
    echo "  → $b"
    # -b  bundle all non-system dependencies
    # -of overwrite files already in dest dir (multiple binaries share libs)
    # -x  binary to fix
    # -d  where to copy the dylibs
    # -p  install-name prefix written into the binary and each dylib
    dylibbundler -b -of \
      -x "$OUT_BIN/$b" \
      -d "$OUT_LIBS/" \
      -p "@executable_path/../libs/"
  done

  echo ""
  echo "━━━ Ad-hoc signing ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
  for f in "$OUT_BIN"/* "$OUT_LIBS"/*.dylib; do
    [[ -f "$f" ]] || continue
    codesign --force --sign - "$f" 2>/dev/null || true
    echo "  ✓ $(basename "$f")"
  done

# ══════════════════════════════════════════════════════════════════════════════
# Linux: bundle .so files
# ══════════════════════════════════════════════════════════════════════════════
elif [[ "$PLATFORM" == linux ]]; then

  command -v patchelf > /dev/null || {
    echo "ERROR: patchelf not found — run: sudo apt install patchelf"; exit 1
  }

  # True for glibc and Linux kernel ABI libs that must stay as host references.
  is_system_so() {
    local name; name="$(basename "$1")"
    [[ "$name" == linux-vdso*    ]] && return 0
    [[ "$name" == ld-linux*      ]] && return 0
    [[ "$name" == libc.so*       ]] && return 0
    [[ "$name" == libm.so*       ]] && return 0
    [[ "$name" == libdl.so*      ]] && return 0
    [[ "$name" == libpthread.so* ]] && return 0
    [[ "$name" == librt.so*      ]] && return 0
    [[ "$name" == libgcc_s.so*   ]] && return 0
    [[ "$name" == libstdc++.so*  ]] && return 0
    return 1
  }

  collect_so() {
    local src="$1"
    while IFS= read -r line; do
      local so; so="$(echo "$line" | awk '/=>/ {print $3}')"
      [[ -z "$so" ]] && continue
      is_system_so "$so"       && continue
      [[ -f "$so" ]]           || continue
      local name; name="$(basename "$so")"
      [[ -f "$OUT_LIBS/$name" ]] && continue
      cp -L "$so" "$OUT_LIBS/$name"
      echo "  + $name"
      collect_so "$OUT_LIBS/$name"
    done < <(ldd "$src" 2>/dev/null)
  }

  echo ""
  echo "━━━ Collecting shared libraries ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
  for b in "${BINARIES[@]}"; do
    [[ -f "$OUT_BIN/$b" ]] && collect_so "$OUT_BIN/$b"
  done

  echo ""
  echo "━━━ Patching RPATH ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
  for b in "${BINARIES[@]}"; do
    [[ -f "$OUT_BIN/$b" ]] || continue
    patchelf --set-rpath '$ORIGIN/../libs' "$OUT_BIN/$b"
    echo "  → $b"
  done
  for sofile in "$OUT_LIBS"/*.so*; do
    [[ -f "$sofile" ]] || continue
    patchelf --set-rpath '$ORIGIN' "$sofile" 2>/dev/null || true
  done

fi

# ── Summary ────────────────────────────────────────────────────────────────
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
printf "  Done!\n"
printf "  Binaries  → %s  (%d files)\n" "$OUT_BIN"  "$(ls "$OUT_BIN"  | wc -l | tr -d ' ')"
printf "  Libraries → %s  (%d files)\n" "$OUT_LIBS" "$(ls "$OUT_LIBS" | wc -l | tr -d ' ')"
echo ""
if [[ "$PLATFORM" == mac ]]; then
  echo "  Next: npm run build:mac"
else
  echo "  Next: npm run build:linux"
fi
