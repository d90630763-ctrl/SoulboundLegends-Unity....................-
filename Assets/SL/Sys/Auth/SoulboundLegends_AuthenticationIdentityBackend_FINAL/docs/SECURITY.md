# Security model

## Passwords
PBKDF2-HMAC-SHA256 with per-password random salt and 210,000 iterations is used. Plaintext passwords are never persisted.

## Access tokens
JWT RS256 access tokens are short-lived and signed by a server-side RSA private key. The private key is never shipped to Unity.

## Refresh tokens
Refresh tokens are high-entropy opaque values. Only SHA-256 hashes are persisted. Refreshing rotates the token. Previous refresh-token hashes are retained as consumed records so reuse can be detected and the session revoked according to policy.

## Google
Google ID tokens are validated server-side using Google's OpenID Connect discovery document and signing keys. The configured audience must match the server's Google client ID. The nonce supplied by the client must match the verified token nonce. Email is not used as the stable provider identity.

## Sessions
A JWT is not sufficient by itself. The `sid` claim must map to an active server session. Revoked/expired sessions fail token validation.

## Account lockout
Repeated password failures increment a server-side counter and temporarily lock the account.

## Administrative operations
Admin endpoints require the `Admin` role from the server-issued token and create security events.

## IP addresses
IP metadata is treated as security metadata, not as an identity credential. Deploy behind a trusted proxy only when the proxy configuration is known and controlled.

## Secrets
Use environment variables or a managed secret store. Never commit:
- Google client secrets
- JWT private keys
- PostgreSQL passwords
- recovery-delivery credentials
