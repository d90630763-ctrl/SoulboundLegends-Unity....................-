# SL Authentication & Identity Backend System — FINAL

Standalone authentication/identity backend for SL.

## What this package is

This is a real server-side authentication service. It is not a Unity-only login screen and it does not simulate Google authentication.

Implemented:
- account creation with PBKDF2 password hashing
- password login
- Google OpenID Connect ID-token verification using Google's discovery/JWKS metadata
- external identity linking/unlinking
- stable account/player/character/session/external-identity identifiers
- signed JWT access tokens
- opaque cryptographically random refresh tokens stored only as hashes
- refresh-token rotation and replay detection
- session timeout, reconnect grace, duplicate-session policies and logout-all
- brute-force/rate limiting
- account lockout
- suspicious-login detection hooks
- audit/security events
- administrative inspection, suspension and forced logout
- recovery challenge persistence and pluggable delivery
- generic [+ Add System] integration contracts
- Unity Runtime client package
- PostgreSQL persistence
- Docker deployment files
- automated unit/integration-oriented tests
- configuration and security documentation

## Important production truth

Generating this ZIP does NOT by itself make a deployed service production-ready.

Before production, the developer/operator must:
1. deploy behind TLS/HTTPS;
2. provision PostgreSQL with restricted credentials;
3. generate and securely store the JWT RSA private key outside source control;
4. configure the Google OAuth/OIDC Web Client ID and authorized redirect/application settings;
5. configure trusted proxy/IP handling for the actual hosting environment;
6. configure secrets using the deployment secret store/environment, not Unity assets;
7. run database migrations/schema deployment;
8. configure monitoring, backups, alerting, log retention and incident response;
9. perform a security review and load/penetration testing;
10. connect the supplied [+ Add System] adapters to the real Account & Session, Player Database, Server Authority, Anti-Cheat, Network Transport, Persistence, Lobby, Character Creation and Master Registry systems.

## External credentials/services the developer must supply

Required for production:
- PostgreSQL database and credentials.
- RSA signing private key for access-token signing.
- Google OAuth/OIDC Web Client ID.
- Hosting TLS certificate/private key or managed TLS service.
- Secret storage / environment variables.

Optional:
- a recovery-delivery service implementing the recovery webhook contract;
- an external player/account registry endpoint;
- centralized audit/analytics sink.

Google client secrets are server-side only. Never put them in Unity.

## Runtime flow

Opening Screen
→ Login
→ Authentication API
→ account/session validation
→ Player/Character identity check
→ Character Creation when no character exists
→ Lobby

The Unity client only requests authentication. The backend is authoritative for identity and session state.
