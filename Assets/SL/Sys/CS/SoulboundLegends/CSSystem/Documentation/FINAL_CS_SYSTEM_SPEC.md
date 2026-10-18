# SL — FINAL CS WORLD SYSTEM

This package is the CS-specific gameplay/world package for SL.

## Core CS identity
CS = Practice & Sharpen.

CS is PURE TEAM ELIMINATION:
- Recommended launch format: 5 vs 5.
- First team to 5 round wins wins the match.
- Maximum 9 rounds.
- A player eliminated during a round does not respawn until the next round.
- No capture points.
- No site-capture objective.
- No Siege objective.
- The seven sites inside a map are combat locations, NOT capture objectives.
- Round victory is decided by elimination.
- If the round timer expires while both teams still have living players, the system can enter pressure/sudden-death handling; elimination remains the deciding objective.

## Adventure World progression connection
CS uses the player's existing Adventure World progression:
- Equipment comes from Adventure World.
- Character power comes from Adventure World.
- Skills come from the player's existing progression.
- Stats/battle power come from the shared progression systems.
- CS does NOT permanently take or destroy Adventure World equipment when the player dies.
- CS should not create a separate permanent combat inventory.

The server must verify ownership and permitted loadout before a player enters a competitive match.

## Five parent maps
The launch configuration contains exactly five parent maps:
1. Verdant Reach — Grasslands
2. Fallen Dominion — Ruins
3. Sunscorch Expanse — Desert
4. Frostveil Range — Snow
5. Abyssal Reach — Ocean

Each parent map has exactly 7 combat sites = 35 sites total.

### Site generation rule
Sites are derived from:
- Parent map name
- Parent map type
- Parent map properties

The generated site identity must stay consistent with the parent environment.

Ocean sites can support:
- Above-water combat
- Floating structures
- Ships/platforms
- Underwater combat
- Sunken structures

Air is included in the type enum and generator hooks for future expansion. It is not one of the five launch parent maps unless you replace a launch category.

## Included CS-specific systems
- CSWorldSystem
- CSMatchManager
- CSTeamManager
- CSRoundManager
- CSScoringSystem
- CSSpawnManager
- CSSpectatorManager
- CSMatchmakingManager
- CSMapSiteManager
- CSBoundaryManager
- CSAFKDisconnectManager
- CSResultRewardManager
- CSServerValidationBridge
- CSIntegrationBridge
- CSGameBootstrap
- CSRulebook
- CSWorldSystemEditor

## Shared system connection slots
The CS World Inspector includes connection slots for:
1. General Lobby System
2. General HUD System
3. Character System
4. FOG Character / Controller System
5. Combat System
6. Skill System
7. Equipment System
8. Inventory / Item System
9. Stats / Battle Power System
10. Animation System
11. Server Authority System
12. Networking / Multiplayer System
13. Account / Player Data System
14. Scene / World Loading System
15. Audio System
16. Camera System
17. Party / Friends System
18. Chat / Communication System
19. Rank / Season System
20. Reward / Progression System
21. Game State System
22. Time Synchronization System
23. Physics / Collision System
24. Localization / Text System
25. Analytics / Match History System
26. Reporting / Punishment System
27. Anti-Cheat System

Required slots are marked in the Inspector. Optional social/analytics features are also exposed so the final package does not need to be rebuilt later.

## Important integration rule
Do not make duplicate Character, Combat, Skill, Equipment, HUD, Lobby, Account, Server, or Networking systems inside the main game just because CS needs them. Assign the existing systems to the CS connection slots.

## Unity installation
1. Open your SL Unity project.
2. Close Unity before copying files.
3. Extract this ZIP.
4. Copy the `Assets/SL/CSSystem` folder into your Unity project's `Assets/SL/` folder.
5. Reopen Unity.
6. Create an empty GameObject named `CS_World_System`.
7. Add `CSWorldSystem`, `CSMatchManager`, `CSTeamManager`, `CSRoundManager`, `CSScoringSystem`, `CSSpawnManager`, `CSSpectatorManager`, `CSMatchmakingManager`, `CSMapSiteManager`, `CSBoundaryManager`, `CSAFKDisconnectManager`, `CSResultRewardManager`, `CSServerValidationBridge`, `CSIntegrationBridge`, and `CSGameBootstrap`.
8. Assign the component references in CSWorldSystem.
9. Use `Create / Reset 5 Default Parent Maps` if the map list is empty.
10. Use `Create / Reset Shared System Connection Slots`.
11. Drag your existing shared systems into the appropriate connection slots.
12. Run `Validate Final CS Configuration`.

## World art and procedural generation
This package defines the gameplay/world architecture and generation data hooks. It does not contain AAA art assets, finished terrain meshes, animations, characters, networking backend credentials, or server infrastructure. Those should be supplied by the corresponding existing/generic systems and connected through the Inspector.

## Security
The included server validation bridge is an integration boundary, not a complete backend service. A production server must authoritatively validate:
- account/player identity
- equipment ownership
- skill ownership
- stats/battle power
- movement/position
- hits/damage
- cooldowns
- deaths
- round score
- match result
- rewards

Never trust a client-provided kill, damage value, score, loadout, or reward request.

## Intended flow
Lobby -> CS matchmaking -> server match instance -> selected parent map/site -> 5v5 elimination rounds -> final result/rewards -> Lobby.

## Shared HUD
The CS mode does NOT create a separate gameplay HUD. It feeds CS-specific information into the existing General HUD:
- Team A / Team B score
- Current round
- Round timer
- Alive players
- Elimination notifications
- Match result
- CS-specific messages

Universal controls remain owned by the General HUD/character/combat systems.

## Future expansion
The architecture allows additional parent map types later, including Air/floating-platform combat, without changing the core elimination rules.
