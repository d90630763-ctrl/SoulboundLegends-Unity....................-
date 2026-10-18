# Setup

1. Install .NET 8 SDK and PostgreSQL 16+.
2. Create a database and a least-privilege application role.
3. Set `SLDB_Database__ConnectionString` and the JWT authority/audience variables. Use TLS in production.
4. From repository root run `dotnet restore`, then `dotnet build`, then `dotnet test`.
5. Run the backend with `dotnet run --project src/Backend`.
6. The startup migration runner applies ordered SQL migrations and rejects checksum drift.
7. Configure your identity provider so server processes and admin users receive appropriate claims/roles.
8. Configure external backup storage. Use `scripts/backup.sh`/`restore.sh` or PowerShell equivalents and test restoration regularly.

## Production checklist
- Use a managed PostgreSQL cluster or equivalent HA setup.
- Store secrets in a secret manager/environment, never in source.
- Put API behind TLS and an appropriate gateway/WAF.
- Restrict database network access to backend services.
- Use separate DB roles for migration and runtime where operational policy requires it.
- Alert on failed migrations, health failures, transaction errors, suspicious mutations and backup failures.
- Test disaster recovery and point-in-time recovery before launch.
- Run load/concurrency testing against your actual deployment topology.
