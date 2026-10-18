# Operations

## Startup
1. Configure `SL_INSTANCE_DATA_DIR`.
2. Configure `SL_INSTANCE_AGENT_URL`.
3. Connect the real authority implementation.
4. Connect the real persistent database implementation.
5. Register specialized gameplay adapters.
6. Register the server fleet heartbeat source.

## Normal lifecycle
A request is validated, capacity is checked atomically, an existing compatible instance is reused when possible, otherwise a suitable server is selected and the runtime agent is asked to provision the instance. Only after provisioning succeeds are players assigned.

## Failure handling
- No capacity: request fails with CapacityUnavailable.
- No healthy server: ServerUnavailable.
- Runtime provisioning failure: CreationFailed/typed exception; cleanup is attempted.
- Assignment failure: instance enters Failed and is persisted.
- Stale server heartbeat: recoverable instances are moved toward controlled draining/reconciliation.
- Destroy with players present: rejected rather than silently deleting player assignments.

## Scale
The included in-memory registry is process-local and should be run as a single authoritative coordinator per consistency domain or replaced with a distributed coordination implementation. Horizontal scaling requires a distributed lock/leader or strongly consistent registry implementation; do not run multiple independent coordinators against the same fleet without that layer.
