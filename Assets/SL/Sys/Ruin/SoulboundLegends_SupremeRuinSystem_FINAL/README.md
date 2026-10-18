# SL Supreme Ruin System — FINAL

## Purpose

This package is the standalone orchestration layer for Supreme Ruins and the five-creature Divine Inheritance trial. It does not replace SL infrastructure, monster generation, combat, instance, persistence, inventory, progression, or networking systems.

## Exact gameplay contract

A valid Supreme Ruin:
1. Is world-connected through a configured entrance/event.
2. Validates every configured requirement server-side.
3. Allows only one active challenger for the specific Ruin.
4. Creates the trial through the connected Dynamic Instance provider.
5. Requests the connected Infrastructure provider to assemble the configured Ruin environment.
6. Creates the Divine Trial through the connected Quest provider.
7. Requires exactly five configured Divine Creatures.
8. Validates the God/element relationship, including Omnigod multi-element rules.
9. Requires server-validated combat death proof for every creature.
10. Awards exactly one configured God inheritance through an atomic inheritance provider.
11. Persists the inheritance through the existing persistence/inheritance architecture.
12. Cleans the instance and world occupancy state.

The system does not reveal Demon World or Shadow God betrayal lore. Lore visibility is delegated to the connected Lore/Discovery provider.

## Dependencies

Required for production deployment:
- Unity 6.x / project-compatible Unity version.
- SL Server Authority provider.
- Server Authorization provider.
- Persistence/database provider for authoritative state.
- Dynamic Instance provider.
- Monster Generation/General Monster provider.
- Combat provider capable of authoritative death proof.
- Character provider.
- World State provider.
- Inheritance provider.
- Infrastructure provider.
- Quest provider.

Optional/configurable:
- Rewards, Inventory, Equipment, Progression, Anti-Cheat, Analytics, UI, Audio, Lore/Discovery, Live Ops, Skills, Skill VFX, Navigation/NPC and other future systems.

No fake database, fake network transport, or fake production server is included.

## [+ Add System] setup

Add the `SupremeRuinConnectionHub` to the same authoritative service object as `SupremeRuinSystem`. Drag compatible provider components into `Connected Systems`. The hub inspects provider capabilities and maps them automatically. A provider may expose multiple capabilities.

The system does not require fixed serialized fields such as `QuestSystem`, `MonsterSystem`, or `CombatSystem`.

## Configuration

Create a `SupremeRuinConfiguration` asset.

For every God define:
- God ID
- God enum
- element
- inheritance ID
- hidden lore key
- Omnigod allowed elements where applicable.

For every Ruin define:
- God
- Kingdom
- entrance
- infrastructure definition
- trial environment
- exact five creature slots
- duration
- failure rules
- reward IDs
- discovery IDs
- spawn points
- party/capacity configuration.

For every Divine Creature define:
- species/template ID
- God
- element
- role
- level
- extreme configurable stat profile
- skill IDs
- resistances
- weaknesses
- AI IDs
- phase IDs
- VFX IDs
- reward IDs.

The default creature stat profile is intentionally extremely high and remains fully configurable.

## Server authority

The client sends intentions only. The server owns:
- access validation
- requirement transaction
- one-challenger lock
- instance creation
- creature spawning
- timer
- creature death validation
- five-kill completion
- inheritance award
- persistence
- cleanup.

Request nonces are validated by the Server Authority provider. Rate limiting is performed before expensive trial creation.

## Requirements

`ISupremeRuinRequirements` provides atomic item checks/consumption plus level, quest, story, world, God condition, permission and discovery checks. The package never invents required items.

Items are consumed only after all requirements have passed and after the server has successfully reserved the Ruin and created/entered the Dynamic Instance. If the transaction fails, the access transaction is rejected before trial state is committed.

Production implementations should make `ConsumeItemsAtomically` transactional and idempotent.

## One-player rule

The established rule is one active challenger per Supreme Ruin. The configuration validator warns if `MaxPlayers` is not 1. The runtime reservation is also server-authoritative through `ISupremeRuinWorld.TryMarkRuinOccupied`.

The Supreme Ruin gameplay rule is exactly one active challenger. Runtime access rejects a configuration that enables party/multiple-player participation for a Supreme Ruin, regardless of the Dynamic Instance provider's broader capabilities.

## Divine Creatures

Exactly five creature slots are required. The runtime rejects a Ruin with any other number.

Every creature must match its God:
- Water → Water
- Fire → Fire
- Earth → Earth
- Void → Void
- Thunder → Thunder
- Nature → Nature
- Light → Light
- Air → Air
- Ice → Ice
- Omnigod → configured allowed elements or MultiElement
- Shadow → Shadow only, while its lore remains controlled/hidden.

The actual monster asset/generation system remains external.

## Failure and cleanup

Failures never award inheritance. The connected Quest provider receives failure state. The Dynamic Instance and Infrastructure providers receive cleanup requests, and world occupancy is released.

The package does not invent inheritance loss or resurrection rules.

## Persistence

`SupremeRuinStateSnapshot` contains schema version, trial/Ruin/God/world IDs, instance, challenger, timing, five-creature progress, participants, inheritance award state, failure state and version.

`InheritanceRecord` contains player/character, God, inheritance, trial/Ruin, acquisition timestamp and schema/version information.

Production persistence must use atomic writes and optimistic/version validation appropriate to the existing SL persistence architecture.

## Anti-cheat

The package reports invalid kill, request and state behavior through Server Authority and the connected Anti-Cheat provider. It does not implement a second anti-cheat engine.

## UI

`ISupremeRuinUi` receives authoritative snapshots. A UI implementation can display:
- Ruin discovery
- requirements
- locked/unlocked state
- current challenger
- Divine Trial
- five-creature progress
- timer
- completion/failure
- inheritance acquisition
- reconnect state.

UI is never a source of gameplay truth.

## Lore

The package does not reveal the Demon World truth or Shadow God betrayal. It only exposes God/creature/trial data to authoritative services. Player-facing lore must be gated by the connected Lore/Discovery provider.

## Live Ops

`ISupremeRuinLiveOps` authorizes administrator commands. Normal clients have no administrative path. Live Ops commands must be authorized server-side by the existing administrative service.

## Testing checklist

- Verify every configured Ruin has exactly five creatures.
- Verify every creature's God/element relationship.
- Verify Omnigod multi-element configuration.
- Verify a second entry request is rejected while occupied.
- Verify duplicate/nonces are rejected.
- Verify requirements fail atomically.
- Verify item consumption occurs only on successful start.
- Verify the Dynamic Instance owns the trial environment.
- Verify the Infrastructure provider creates and destroys the environment.
- Verify each creature death requires Combat proof.
- Verify fake client kill requests fail.
- Verify four kills never complete the trial.
- Verify the fifth legitimate kill completes it.
- Verify inheritance is awarded exactly once.
- Verify a second inheritance request fails.
- Verify inheritance owner cannot start another trial.
- Verify cleanup occurs after success and failure.
- Verify persisted state restores after server restart.
- Verify disconnect/reconnect cannot reset progress.
- Verify failure never awards inheritance.
- Verify anti-cheat receives invalid actions.
- Verify hidden Demon World/Shadow lore is not exposed early.
- Verify UI cannot mutate server state.
- Verify the server remains authoritative under simultaneous entry requests.

## Completion transaction boundary

The fifth legitimate creature death is only the final combat gate. The Quest provider must accept completion before the inheritance transaction. The inheritance provider then performs the permanent, atomic one-inheritance grant. Optional additional rewards are independent and must be idempotent/retryable; their failure cannot revoke or falsify an already-awarded inheritance.

## Participant/assist enforcement

`CanEnterOrAssist` is an authoritative gate for Dynamic Instance, portal, teleport and other participant-join paths. The connected instance/portal services should invoke it before admitting any player. The only accepted participant is the active challenger.
