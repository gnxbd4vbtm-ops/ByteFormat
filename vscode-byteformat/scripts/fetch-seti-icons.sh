#!/usr/bin/env bash
set -euo pipefail

# Fetch Seti UI icon files into icons/seti/
# This script clones a known Seti UI repository and copies icon files.
# Run from the vscode-byteformat folder: `bash scripts/fetch-seti-icons.sh`

OUT_DIR="icons/seti"
TMP_DIR=".tmp_seti_repo"
REPO_URL="https://github.com/jesseweed/seti-ui.git"

echo "Fetching Seti UI icons from ${REPO_URL}..."
rm -rf "${TMP_DIR}" "${OUT_DIR}"
git clone --depth 1 "${REPO_URL}" "${TMP_DIR}"
mkdir -p "${OUT_DIR}"

# Copy common icon files (adjust paths if upstream layout differs)
if [ -d "${TMP_DIR}/icons" ]; then
  cp -R "${TMP_DIR}/icons/"* "${OUT_DIR}/"
elif [ -d "${TMP_DIR}/dist" ]; then
  cp -R "${TMP_DIR}/dist/"* "${OUT_DIR}/"
else
  echo "Warning: couldn't find expected icon folders in the cloned repo. Inspect ${TMP_DIR} manually." >&2
fi

# Copy a short license/attribution note
cat > "icons/SETI_LICENSE.txt" <<EOF
Seti UI icons fetched from ${REPO_URL}
Please review the upstream repository for exact license terms and attribution requirements.
EOF

echo "Seti icons copied to ${OUT_DIR}. Review icons/SETI_LICENSE.txt for attribution details."
rm -rf "${TMP_DIR}"
