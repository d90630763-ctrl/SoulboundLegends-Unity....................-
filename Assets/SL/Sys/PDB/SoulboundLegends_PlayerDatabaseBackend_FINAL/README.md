# SL Player Database Backend — FINAL

Standalone persistent backend for SL. It is designed to be integrated later with the Combined Unity project without making Unity authoritative over persistent state.

## Implemented
- ASP.NET Core .NET 8 backend service structure.
- Provider abstraction (`IDatabaseProvider`) plus PostgreSQL/Npgsql implementation.
- Connection factory/pooling via Npgsql connection pooling.
- Repository + service + transaction boundaries.
- Parameterized SQL only.
- Optimistic concurrency using `version` columns.
- Idempotency records for mutation de-duplication.
- Durable economy/inventory ledger operations with database transactions and row locks.
- Schema migration runner with checksum and migration history.
- Validation, sanitized error responses, retry policy, timeout settings, health checks.
- Audit/security mutation logging.
- Backup/restore operational interfaces and scripts; actual storage/cloud redundancy remains deployment infrastructure.
- Unity client SDK with cache invalidation, retries, authentication relationship and server-authority contract.
- Generic `+ Add System` capability connections on the Unity side.
- Supreme Ruin and Rift state persistence with invariant enforcement.
- Admin service endpoints gated by an authority context.
- Critical unit/integration-test source covering idempotency, validation, version conflicts, state transitions and economy invariants.

## External dependencies that must be supplied at deployment
- .NET 8 runtime/SDK.
- PostgreSQL 16+ (recommended) and its durable storage.
- Real authentication/identity provider issuing trusted server-verifiable JWTs.
- Production secret store / environment variables.
- Production backup destination and restore operator controls.
- TLS termination/certificates.

No production password, API key, signing key, or cloud credential is contained in this package.

## Important honesty note
This archive is source-complete, but it has not been compiled inside this execution environment because a .NET SDK/compiler is not installed here. Run the documented `dotnet restore`, `dotnet build`, and `dotnet test` commands in a .NET 8 environment before deployment. The package does not claim that generated source alone equals a production deployment.
