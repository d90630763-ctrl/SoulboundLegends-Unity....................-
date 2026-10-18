# Authentication API

Base path: `/api/v1/auth`

## Public
- `POST /register`
- `POST /login/password`
- `POST /login/google`
- `POST /refresh`
- `POST /recovery/request`
- `POST /recovery/complete`

## Authenticated
- `POST /logout`
- `GET /session`
- `GET /identities`
- `POST /identities/google`
- `DELETE /identities/{provider}`

## Admin
- `GET /api/v1/admin/accounts/{accountId}`
- `GET /api/v1/admin/accounts/{accountId}/sessions`
- `GET /api/v1/admin/accounts/{accountId}/security-events`
- `POST /api/v1/admin/accounts/{accountId}/force-logout?reason=...`
- `POST /api/v1/admin/accounts/{accountId}/suspension?suspended=true|false`

## Authentication boundary

Every trusted gameplay connection must present a valid access token. The server validates:
- signature
- issuer
- audience
- expiration
- account ID (`sub`)
- session ID (`sid`)
- active session record

The Unity client must not send an account/player ID and expect the server to trust it. The authenticated identity comes from the verified token/session.

## Character transition

The authentication response contains:
- `accountId`
- `sessionId`
- `characterExists`
- `playerId`
- `characterId`

If `characterExists=false`, the game enters Character Creation. Character Creation then calls the Player Database / Character Generation systems and attaches the resulting server-created player/character IDs through the integration contract.
