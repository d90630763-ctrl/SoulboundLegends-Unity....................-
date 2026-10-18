# SL — Rift System FINAL

## What this package is

This package implements the authoritative Rift world-event domain for SL. A Rift is a rare world event attached to an Adventure World city/kingdom, not an ordinary dungeon.

The package includes:
- Explicit server-side Rift state machine.
- Server-authoritative creation, entry, attempts, timers, boss defeat validation, completion, engulfment and destruction commit flow.
- Persistent Rift snapshots with optimistic version tokens.
- Generic `[+ Add System]` capability-based integration hub.
- Dynamic Instance integration contract rather than a second instance framework.
- Existing Monster/Combat/Skill/VFX/Character contracts rather than duplicated gameplay systems.
- Quest/Guild, Lore/Discovery, Rewards, Progression/Punishment, Kingdom, NPC, Navigation, Portal, Anti-Cheat, Analytics, UI, Audio and Live Ops contracts.
- Reconnect handling.
- Mobile-oriented population, snapshot, and VFX configuration.
- Editor validation tool.
- Development-only local authority provider.

## Unity version

Target: Unity 6000.3 or newer compatible Unity 6 LTS release.

## Dependencies

This package has **no hard third-party Unity package dependencies**.

The following are external/Combined-Project dependencies for production multiplayer operation:
1. A real authoritative multiplayer/network transport/server runtime.
2. A real Server Authority provider.
3. A real Server Authorization provider.
4. A persistent database or server persistence provider.
5. The existing SL Dynamic Instance system.
6. Adventure World / World Kingdom / World Events.
7. Monster Generation / General Monster.
8. Combat / General Skill / Skill VFX / Character.
9. Destruction / Portal / Navigation / NPC.
10. Inventory / Equipment / Rewards / Progression.
11. Anti-Cheat / Analytics / Lore / Notifications / Audio / UI / Live Ops.

The package deliberately does not pretend those external systems are included.

## Installation

1. Back up the Combined SL project.
2. Copy this folder into `Packages/com.soulboundlegends.rift-system`.
3. Let Unity import and compile the package.
4. Create a `RiftConfiguration` asset:
   `Assets > Create > SL > Rift > Rift Configuration`.
5. Create a GameObject with:
   - `RiftSystemConnectionHub`
   - `RiftSystem`
   - optionally `RiftServerApi`
   - optionally `RiftLiveOpsService`
6. Assign the configuration and connection hub.
7. Add connected provider components to the hub's `Connected Systems` list using the Inspector's normal `+` list control. This is the package's generic `[+ Add System]` architecture.
8. Run `SL > Rift > Validate Setup`.

## [+ Add System] architecture

`RiftSystemConnectionHub` accepts arbitrary MonoBehaviours implementing `IRiftSystemProvider`.

Each provider declares:
- Provider ID
- Priority
- Capabilities
- Availability

The Rift System asks the hub for capabilities at runtime. It does not hard-code one implementation of Quest, Monster, Inventory, Database, etc.

Multiple providers can expose the same capability. Highest priority is selected for single-provider requests; `GetAll<T>` is available for broadcast-style integrations such as analytics.

A future system can be added by implementing the appropriate provider contract and adding its component to the connection list. The Rift System itself does not need to be rebuilt.

## Rift state machine

Normal
→ Appearing
→ Active
→ InProgress
→ BossActive
→ Cleared

Failure/escalation:
Active/InProgress/BossActive
→ TimeExpired
→ Merging
→ Engulfed
→ Cleared OR CityDestroyed

or:
Active/InProgress/BossActive
→ AttemptsExhausted
→ Merging
→ Engulfed
→ Cleared OR CityDestroyed

Illegal transitions are rejected by `RiftStateMachine`.

## Server authority

The production server must supply `IRiftServerAuthorityProvider`.

The client must not own:
- Rift creation
- Rift state
- timers
- attempts
- monster/boss spawning
- boss defeat
- rewards
- engulfment
- city destruction
- completion/failure

Player requests pass through `RiftAuthorityGuard`, which validates:
- authoritative server execution
- rate limiting
- request nonce
- connected Server Authorization capability when present

The existing Anti-Cheat system receives anomaly reports rather than being duplicated here.

## Persistence

Production persistence must implement `IRiftPersistenceProvider` or `IRiftDatabaseProvider`.

Snapshots include:
- schema version
- Rift ID
- type/difficulty
- kingdom/city
- state
- failure
- timestamps
- attempts
- participants
- instance
- monster/boss state
- world consequence
- completion/destruction/reward flags
- optimistic version token

Use the version token for compare-and-swap style persistence so two authoritative writers cannot silently overwrite one another.

## Adventure World integration

`IRiftAdventureWorldProvider` supplies:
- valid city selection
- world condition validation
- server world-state notifications

The Rift never chooses an arbitrary unrelated dungeon location.

## Quest integration

`IRiftQuestProvider` receives the Rift snapshot.

The intended UI values are:
- Rank: `???`
- Objective: `KILL THE FINAL BOSS.`

The provider owns the actual Quest/Guild implementation.

The Rift System does not expose the Demon World truth.

## Monster integration

Rift monsters are requested through `IRiftMonsterProvider`.

The Rift System does not implement a second monster generator, monster AI, combat engine, or skill engine.

Rift wave configuration supplies monster definition IDs, spawn points, levels, elite/boss flags and wave numbers. The existing Monster/General Monster system remains responsible for constructing and controlling the actual creature.

## Combat / boss integration

Boss defeat is accepted only when `IRiftCombatProvider.ValidateBossDefeat` confirms the server-side combat result.

The Rift System does not trust a client claim that a boss died.

## Dynamic Instance integration

`IRiftInstanceProvider` owns:
- instance creation
- capacity
- party permission
- moving players
- removing players
- destruction

The Rift package does not create a second unrelated instance framework.

## City engulfment and destruction

On merging:
- the Rift marks the city as engulfed through World Kingdom.
- Navigation/NPC providers restrict access.
- inside participants are server locked.
- invasion monsters are requested from the existing Monster system.

The connected Destruction system is the authority that determines whether the city has actually been destroyed. Only after it commits that world destruction does the Rift enter `CityDestroyed`.

The Kingdom provider must persist the permanent city destruction. This survives logout, reconnect, client restart and server restart.

No resurrection/reconstruction mechanic is included.

## Player punishment

When destruction is committed, the Rift emits a punishment context to `IRiftProgressionProvider`.

The Rift System does not invent punishment values.

## Reconnect

`TryRestorePlayer` restores:
- participant association
- instance placement
- lock state when the city is engulfed
- server timestamp

Reconnect does not reset attempts, timers, state, boss state, or world consequences.

## Lore / Discovery

The Rift exposes discovery events through `IRiftLoreProvider`.

The initial discovery is deliberately generic:
`rift.appeared`

The package never requires the client to know the Demon World connection.

The connected Lore/Discovery system can unlock later clues from:
- encounters
- monsters
- bosses
- completions
- failures
- world progression

## UI

`RiftUiViewModel` provides server snapshot delivery and safe display helpers.

Required UI surfaces supported by the snapshot architecture:
- discovery notification
- Rift quest
- `???` rank
- city/kingdom
- remaining time
- remaining attempts
- state
- boss objective
- participants
- engulfment warning
- destruction warning
- completion/failure/rewards
- reconnect
- boundary warning

UI values are read-only projections. They do not control the Rift.

## Mobile optimization

Configuration supports:
- maximum invasion population
- snapshot interval
- VFX prewarm count
- instance capacity
- capped wave populations

AOI and network replication remain the responsibility of the existing multiplayer/AOI architecture. The Rift only emits authoritative state changes.

## Audio/VFX

Use `IRiftAudioProvider` and `IRiftSkillVfxProvider`.

The Rift System does not replace the existing VFX or Audio systems.

The connected systems can map:
- Rift appearance
- ambience
- warnings
- engulfment
- monster/boss effects
- destruction
- clear/collapse

to their own pooled assets.

## Live Ops/Admin

`RiftLiveOpsService` accepts commands only through the connected Live Ops authorization capability.

Normal player requests do not have administrative access.

Production admin tooling should call the Rift service through the server-authorized admin channel. Admin operations are exposed only through the server-side `RiftLiveOpsService` and require the connected `IRiftLiveOpsProvider` to authorize every command.

## Analytics

The package emits server events for:
- spawned
- discovered
- entered
- left
- attempt consumed
- instance created/destroyed
- boss spawned/defeated
- cleared
- timer expired
- attempts exhausted
- merged
- engulfed
- city destroyed
- rewards
- reconnect
- persistence failure

Analytics providers receive those events.

## Error handling

The Rift system refuses to:
- grant rewards without a rewards provider
- accept fake boss defeat
- commit city destruction without the Destruction provider
- silently substitute duplicate external systems
- accept client authority

If required infrastructure is unavailable, the operation fails instead of fabricating success.

## Testing checklist

### Build
- Package imports without compiler errors.
- Both assembly definitions resolve.

### Configuration
- RiftConfiguration exists.
- At least one RiftTypeDefinition exists.
- Monster and boss IDs resolve in the connected Monster system.
- Spawn point IDs resolve in the connected world/instance system.

### Authority
- Production Server Authority provider reports authoritative server.
- Client cannot instantiate the production authority provider.
- Server Authorization rejects unauthorized requests.
- Nonces cannot be replayed.
- Rate limits operate.

### Spawn
- Server creates Rift.
- City belongs to Adventure World.
- Quest shows `???`.
- Objective is `KILL THE FINAL BOSS.`.
- No Demon World truth is injected.

### Instance
- Instance created through existing Dynamic Instance system.
- Capacity is enforced.
- Party permission follows configuration.

### Attempts/timer
- Attempts are server incremented.
- Timer is server calculated.
- Restart restores attempts and expiration.
- Client cannot reset either.

### Boss
- Boss comes from connected Monster system.
- Combat provider validates actual defeat.
- Client cannot claim boss death.

### Success
- Rift becomes Cleared.
- City engulfment is removed when applicable.
- Rewards are granted once using idempotency keys.
- Quest completes.
- Instance cleans up.

### Failure
- Timer expiration enters Merging.
- Attempts exhaustion enters Merging.
- Engulfment is persistent.
- Inside players remain locked.
- Outside players/NPCs cannot bypass the boundary through connected Portal/Navigation systems.
- Invasion uses existing Monster system.
- Destruction provider commits permanent city destruction.
- Progression provider receives punishment contexts.

### Persistence/reconnect
- Restart restores Rift.
- Reconnect restores participant state.
- Engulfed players remain locked.
- City destruction remains after restart.

### Security
- Impossible movement is reported.
- Impossible exit is reported.
- Fake completion is rejected.
- Fake rewards are rejected.
- Unauthorized creation is rejected.
- Admin commands require Live Ops authorization.

## Important production boundary

This package is the complete Rift domain implementation and integration contract, but a standalone package cannot truthfully contain your production server, database, authentication, network transport, or the already-existing SL systems.

Those are explicitly declared interfaces/dependencies. The Combined project must provide their implementations.

The included `RiftLocalTestAuthority` exists only for local/editor testing and is deliberately marked as non-production.

## Boundary enforcement

Production movement, portal and teleport services should call `RiftSystem.ValidateBoundaryEntry`, `ValidateExitBoundary`, and `ValidatePortalUse` from their authoritative request paths. The Rift package therefore supplies the authoritative decision point without replacing the existing movement/portal systems. Clients must never be the final authority.

## Invasion/destruction boundary

The connected Destruction provider owns the actual city-health/destruction condition. The Rift domain records permanent destruction only after that provider commits the destruction. This prevents the Rift system from inventing a duplicate city-destruction engine.

## Invasion wave schedule

After engulfment, waves are not spawned in one burst. The server persists `NextWaveUnixMilliseconds` and `SpawnedWaveNumbers`, then advances configured non-boss waves according to each wave's `DelaySeconds` while enforcing the mobile population cap. Boss spawning remains part of the normal Rift content progression and is validated by Combat before completion.

## Destruction provider contract

`IRiftDestructionProvider.EvaluateWorldDestruction` is a condition check. It must return `true` only after the connected World/Destruction system has authoritatively determined that the city has actually been destroyed. It must not destroy the city simply because the Rift asks for a check. The Rift system then records the permanent consequence through `IRiftKingdomProvider.TrySetCityDestroyed`.
