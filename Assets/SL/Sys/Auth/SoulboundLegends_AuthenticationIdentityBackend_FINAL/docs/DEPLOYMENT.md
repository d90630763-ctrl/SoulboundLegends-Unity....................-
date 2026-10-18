# Deployment

## Generate an RSA key

Use an offline/admin-controlled machine and store the private key in a secret manager. Example:

`openssl genpkey -algorithm RSA -pkeyopt rsa_keygen_bits:3072 -out sbl-jwt-private.pem`

Do not commit this file.

## Database

Create a PostgreSQL database and run `database/schema.sql`, or use a controlled EF migration pipeline after adding your environment's migration history.

## Environment

Set:
- `ConnectionStrings__Postgres`
- `Authentication__Issuer`
- `Authentication__Audience`
- `Authentication__GoogleClientId`
- `SBL_JWT_PRIVATE_KEY_PEM`

For production, also configure recovery delivery and external integration URLs as appropriate.

## TLS

Expose the service only through HTTPS. The sample Docker service listens internally on HTTP so a reverse proxy can terminate TLS.

## Scaling

All identity state is stored in PostgreSQL. Multiple auth service instances can therefore share account/session state. Rate limiting in this sample is per process; for a multi-instance production deployment, put distributed rate limiting at the gateway/API layer.

## Operational requirements

Use:
- managed PostgreSQL or HA PostgreSQL
- encrypted backups
- secret manager
- central logs
- alerting
- metrics
- WAF/API gateway as appropriate
- key rotation plan
- incident response plan
- security testing
