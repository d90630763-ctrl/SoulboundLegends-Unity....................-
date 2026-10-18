#!/usr/bin/env bash
set -euo pipefail
: "${DATABASE_URL:?DATABASE_URL is required}"
backup="${1:?backup file required}"
pg_restore --clean --if-exists --no-owner --dbname="$DATABASE_URL" "$backup"
