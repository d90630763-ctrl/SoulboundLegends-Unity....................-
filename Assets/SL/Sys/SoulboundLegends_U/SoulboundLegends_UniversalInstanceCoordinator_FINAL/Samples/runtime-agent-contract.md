# Runtime Agent contract

The coordinator calls the actual authoritative runtime manager:

POST /instances/provision
Body: complete InstanceRecord JSON.
Expected: 2xx only after the runtime has accepted/provisioned the instance identity and server placement.

POST /instances/deprovision
Body: complete InstanceRecord JSON.
Expected: 2xx after the runtime has stopped/released the instance.

GET /instances/{instanceId}/health
Expected: 2xx while the authoritative runtime is alive and owns the instance.

The runtime agent must authenticate server-to-server calls, verify the coordinator's authority, enforce server-local capacity, and reject conflicting instance IDs.
