# SL Universal Instance Coordinator — FINAL

## Purpose
The Universal Instance Coordinator is the central orchestration layer for SL instances. It does **not** own the gameplay of villages, kingdoms, dungeons, Rift, Supreme Ruin, CS, Battle Royale, Siege, or future modes. Those systems connect through gameplay adapters/capability providers.

## Authority boundary
The server is authoritative for instance ID, server assignment, capacity, reservations, player assignment, lifecycle, transfer state, and recovery. Clients never choose authoritative instance/server ownership.

## Lifecycle
`Requested -> Validated -> CapacityChecked -> ServerSelected -> InstanceCreated -> PlayersAssigned -> Running -> Draining -> Destroyed -> CleanupComplete`.
Failures are represented by `Failed` plus a typed `FailureState` and reason.

## Reuse and sharding
A compatible running instance is reused when enough capacity exists. Party size is treated as one atomic capacity requirement. If the party cannot fit, a different instance/server is selected. This supports Village 001/002/003, kingdom/city shards, inns, shops, taverns, events and similar dynamic instances without putting the entire player population in one world process.

## Specialized systems
Register an `IGameplayAdapter` on the server for each gameplay owner. The adapter receives validation/create/assignment/drain/destroy callbacks. The coordinator does not implement the rules for Rift engulfment, Supreme Ruin inheritance, CS scoring, BR rules, Siege rules, dungeon encounters, etc.

## Actual provisioning
The coordinator calls an authoritative **Instance Runtime Agent** through `HttpRuntimeProvisioner`. That agent is responsible for creating the actual gameplay runtime/process/container and exposing:
- POST `/instances/provision`
- POST `/instances/deprovision`
- GET `/instances/{instanceId}/health`

The package deliberately does not fabricate an instance by creating a Unity GameObject. This boundary is required for real distributed deployment.

## Persistence
`JsonFilePersistence` is included as a durable local/reference implementation and stores instance records plus audit data. For production scale, implement `IInstancePersistence` against the project's authoritative database. Real-time state belongs in the coordinator/server memory; the database is for durable instance metadata, persistent-world relationships, audit and recovery metadata.

## Configuration
Set `SL_INSTANCE_DATA_DIR` for durable coordinator data and `SL_INSTANCE_AGENT_URL` for the authoritative runtime agent. Do not expose the coordinator directly to untrusted clients; place it behind the game's authenticated server-to-server network and authorization layer.

## Unity installation
Copy the `Unity` folder into the combined project or install `Unity/package.json` as a local package. Create an `InstanceCoordinatorConfig` asset and a `UniversalInstanceCoordinator` GameObject. Supply an `IInstanceCoordinatorBackend` implementation pointing to the authoritative coordinator API. Connect future systems through `SystemConnector` using `ISystemCapabilityProvider`.

## Required integrations
Network Transport, Master Authority, Server Authorization, Anti-Cheat, Database/Persistence, AOI, Matchmaking, Adventure World, CS, BR, Siege, Rift, Supreme Ruin, Dungeon/Event, Infrastructure, Destruction, Analytics and LiveOps remain separate systems. They connect through capability providers/adapters rather than hard-coded dependencies.

## Crash recovery
The coordinator accepts server heartbeats, persists lifecycle metadata, reloads recoverable records after restart, and marks instances owned by stale/offline servers as `Draining`/`RecoveryFailed` for controlled reconciliation. Player recovery is performed by the authoritative transport/session layer; the coordinator does not invent player state.

## Mobile performance
The Unity side is event-driven and keeps only a lightweight local registry. It does not poll per-player state. Capacity/load calculations are server-side.

## Testing
- Unity model tests cover request/capacity/lifecycle fundamentals.
- Server integration test exercises creation, shard reuse, player release and destruction with fake infrastructure injected only into the test process.
- Production runtime provisioning is deliberately tested through the runtime-agent HTTP boundary rather than a fake local GameObject.

## Production qualification
This ZIP is a complete implementation of the coordinator layer and its documented integration boundaries. It is **not by itself a production deployment of SL**: production qualification still requires connecting the project's real Master Authority, authentication, anti-cheat, database, network transport, server fleet/runtime agent, observability and specialized gameplay systems, then load-testing and security-testing those integrations.
