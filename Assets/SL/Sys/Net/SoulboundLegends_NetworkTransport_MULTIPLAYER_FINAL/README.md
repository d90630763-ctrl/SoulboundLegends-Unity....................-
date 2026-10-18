# SL Multiplayer Network Transport — FINAL

## What this package is
A standalone, concrete multiplayer transport layer for Unity 6 / .NET. It contains a working UDP transport implementation, reliable/ordered channels, packet framing, acknowledgements, sequence tracking, heartbeat, RTT/loss metrics, HMAC authentication, reconnect tokens, rate/size limits, replication filtering, AOI hooks, instance isolation, server-authority request routing, and a separate .NET server host.

## Important truth about production readiness
This is a production-structured implementation, not a claim that a generated ZIP is automatically production-ready for millions of users. Before live launch, you must load-test the chosen hosting/network topology, deploy TLS/key management or equivalent secure credential delivery, integrate the game's real authentication/database/anti-cheat/authority services, tune MTU/rates, add DDoS protection, and run platform-specific certification/testing. The package deliberately exposes those integrations instead of pretending they can be safely hard-coded.

## External dependencies
- Unity 6.5 / 6000.5+ compatible (package does not require Unity Netcode).
- .NET 8 SDK for the standalone server host.
- NUnit is only required to run the optional Unity/server test assemblies.
- No third-party transport provider is required: the included `UdpReliableTransportProvider` is the concrete provider.

## Architecture
Client gameplay systems -> `INetworkTransportProvider` -> concrete UDP provider -> packet protocol.
Server gameplay/authority systems -> `ServerNetworkHost` -> UDP provider -> clients.

The transport never grants client authority. Gameplay requests enter `ServerAuthorityPipeline`, which requires authentication, security/rate checks and an authority validator before the supplied state-change delegate is invoked.

## Unity install
1. Copy the folder into `Packages/com.soulboundlegends.networktransport.multiplayer` or add it as a local package.
2. Add `NetworkTransportManager` to a bootstrap GameObject.
3. Assign `NetworkTransportConfig` and choose `UDP Reliable`.
4. Set the server endpoint and authentication configuration through a secure deployment configuration source.
5. Connect existing [+ Add System] capability providers through `NetworkSystemRegistry` or `NetworkIntegrationHub`.

## Server
From `Server/` run `dotnet restore`, then `dotnet run -- --config server.json`.
The sample server accepts authenticated UDP clients using a configured shared secret and exposes authority/replication integration points. It is intentionally a host, not a replacement for your Master Authority, database, matchmaking or anti-cheat services.

## Channels
- ReliableOrdered: gameplay requests, important state changes, inventory/social events.
- ReliableUnordered: reliable events where ordering is not required.
- UnreliableSequenced: high-frequency snapshots/movement.
- Unreliable: disposable telemetry/effects.

## Security
Packets are length-checked, magic/version checked, sequence checked, replay-window checked, rate limited, authenticated after handshake, and optionally HMAC signed. Client movement is an input/request, never authoritative position. Damage/rewards/currency/progression/Rift/Ruin outcomes must be produced by server-side systems.
