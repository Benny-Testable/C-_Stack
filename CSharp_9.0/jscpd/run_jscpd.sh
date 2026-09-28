#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")" && pwd)"
cd "$ROOT"

if ! command -v npx &>/dev/null; then
  echo "jscpd requires node/npx — install Node.js to enable." >&2
  exit 1
fi

npx --yes jscpd . --pattern "**/*.cs" --min-lines 10 --min-tokens 50 --reporters console,json --output ./report
