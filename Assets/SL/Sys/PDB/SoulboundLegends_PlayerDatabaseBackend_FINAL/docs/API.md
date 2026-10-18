# API

`GET /health` — liveness/DB health.

`GET /api/v1/player/{playerId}` — authenticated player snapshot; ownership is checked server-side.

`POST /api/v1/player/{playerId}/mutation` — authenticated persistent mutation contract. Body: `operation`, `idempotencyKey`, `expectedVersion`, `payload`. Supported operations are deliberately allow-listed.

Admin endpoints inspect player, economy transactions, Rift, Ruin and migration status and require the `admin` or `operations` role.

Do not expose direct SQL endpoints to clients.
