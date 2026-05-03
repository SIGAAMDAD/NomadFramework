#!/usr/bin/env bash
set -euo pipefail
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
dotnet run --project "$REPO_ROOT/Tools/Nomad.ApiSurfaceValidator/Nomad.ApiSurfaceValidator.csproj" -- --repo-root "$REPO_ROOT" "$@"
