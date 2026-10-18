# Test matrix

Automated coverage included:
- valid password hash/verify
- plaintext password is not stored by the password hasher
- refresh-token randomness
- refresh-token deterministic hashing
- security requirement contract checks

Server integration scenarios that should be executed in CI/staging against PostgreSQL and Google test credentials:
- valid login
- invalid login
- expired access token
- revoked session
- refresh rotation
- refresh replay
- duplicate login
- logout
- logout-all-devices
- reconnect within grace period
- account creation
- Google provider failure
- unauthorized identity
- brute-force lockout
- session invalidation after recovery
- admin authorization
- admin audit event generation

These scenarios are API-level acceptance tests rather than simulated UI tests.
