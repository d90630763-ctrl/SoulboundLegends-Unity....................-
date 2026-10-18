#!/usr/bin/env bash
set -euo pipefail
: "${DATABASE_URL:?DATABASE_URL is required}"
out="${1:-soulbound_$(date +%Y%m%d_%H%M%S).dump}"
pg_dump --format=custom --no-owner --file="$out" "$DATABASE_URL"
pg_restore --list "$out" >/dev/null
