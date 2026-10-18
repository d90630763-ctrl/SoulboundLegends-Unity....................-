# Security boundary

1. The Unity client is never trusted for instance ID, server ID, capacity, ownership or world state.
2. The coordinator API must be server-to-server authenticated in production. The included `AllowAllAuthority` exists only as a replaceable local implementation and must not be used for an internet-facing deployment.
3. Runtime-agent calls must use mutually authenticated service credentials or the project's equivalent secure service identity.
4. Player transfer confirmation must be authenticated by the authoritative session/transport service.
5. Anti-cheat remains a separate authority. The coordinator only orchestrates instance placement and lifecycle.
6. Persist audit events and recovery metadata in the production database; do not treat client logs as authoritative.
7. Rate-limit create/transfer requests at the gateway and enforce per-account/session authorization before they reach the coordinator.
