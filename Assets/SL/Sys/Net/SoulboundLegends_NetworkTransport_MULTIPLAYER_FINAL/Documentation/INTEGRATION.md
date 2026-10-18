# Integration map

## [+ Add System]
`NetworkSystemRegistry` accepts capability systems. Add implementations for Master Authority, Server Authorization/Security, Authentication, Anti-Cheat, Analytics, Instance Coordinator and Matchmaking without changing the transport.

## Authority flow
Send a `Request` message. On the server call `ServerAuthorityPipeline.Process`. It checks rate limit, authenticated state, anti-cheat inspection, then the authoritative validator. Only after validation should the game state mutate and replication send the result.

## Movement
Transmit compact input/state requests on `UnreliableSequenced`. The server simulates/validates movement and sends authoritative snapshots. Client-side prediction/interpolation belongs above transport; transport exposes ordering, sequencing and RTT needed by that layer.

## Combat
Attack/skill/hit requests use reliable ordered messages where outcome ordering matters. Server computes hit/damage/death/reward. Client never submits authoritative damage, currency, inventory, XP, inheritance, Rift/Ruin result, or reward state.

## AOI/instances
Implement `IAoiProvider` using System14 Multiplayer AOI. Implement `IInstanceContextProvider` using Universal Instance Coordinator/Dynamic Instance systems. `ReplicationScheduler` only builds state from the viewer's instance and AOI result. The configured visible-entity ceiling defaults to 35.

## Matchmaking
System24 should assign a server/instance before the client connects. Match assignment and cancellation are service messages; loading/ready is transport traffic. On reconnect, the authoritative instance service restores the session and then replication resynchronizes.
