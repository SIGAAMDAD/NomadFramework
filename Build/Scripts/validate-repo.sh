#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"

"$ROOT_DIR/Build/scripts/validate-manifest.sh" "$@"
"$ROOT_DIR/Build/scripts/validate-api-surface.sh" "$@"
