# 14 — Multiplayer AOI System

## Final implementation target
This package is part of the 45-system architecture. Its implemented responsibility is:

**grid AOI, dynamic movement, enter/exit, visibility cap, synchronization and optimization**

## Dependencies
Primary architecture dependencies: **43, 44**.

The dependency names are integration contracts. This ZIP does not silently assume a third-party networking package.

## Authority rule
For multiplayer gameplay, the client sends intent. The authoritative server validates identity, permission, ownership, state, timing, distance, resources and rate limits before changing persistent gameplay state.

## Unity
Designed for Unity 2022.3+ / Unity 6. Runtime code uses UnityEngine plus standard C# only unless a dependency is explicitly documented.

## Validation
The config has an Inspector validation action. The package also contains request/result contracts and a network adapter interface so it can be connected to the Server Authority Core and Network Adapter Contracts.

## Installation
Copy the `Assets/GameSystems/MultiplayerAOI` folder into the Unity project's `Assets/GameSystems/` directory.

## Important
This is implementation code and integration contracts, not a claim that a particular external server provider, cloud database, payment provider, or networking library is bundled. Those provider-specific pieces must be connected through the interfaces documented in the source.
