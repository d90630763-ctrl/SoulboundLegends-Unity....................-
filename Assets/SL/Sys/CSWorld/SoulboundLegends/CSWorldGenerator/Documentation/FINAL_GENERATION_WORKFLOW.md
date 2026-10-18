# FINAL CS WORLD GENERATION WORKFLOW

## What this version does

This version changes the CS package from a passive collection of managers into a **generation orchestrator**.

You connect the systems you already made, especially:
- Terrain Generator
- Infrastructure Generator
- Water/Environment
- Material/Texture
- Weather/Lighting
- Terrain Destruction
- Character/Monster/NPC systems where desired
- General HUD
- Character
- Combat
- Skills
- Equipment
- Stats/Battle Power
- Server Authority
- Networking
- Account
- Scene Loading
- Anti-Cheat
- and the other shared systems.

Then press:

**GENERATE FINAL CS WORLD**

The generator:
1. Validates required connections.
2. Creates the CS World root.
3. Creates exactly five launch parent maps.
4. Creates exactly seven combat sites for each parent map.
5. Derives each site's identity from the parent map name/type/properties.
6. Creates site generation instructions for Terrain and Infrastructure.
7. Calls every connected system that implements `ICSWorldGenerationProvider`.
8. Keeps all other connected systems as dependencies for the generated world.
9. Applies the fixed CS rules.
10. Creates spawn and combat-boundary data.
11. Creates the shared HUD integration data.
12. Creates server-validation integration data.
13. Produces a generation report.

## Five launch maps

1. Verdant Reach — Grasslands
2. Fallen Dominion — Ruins
3. Sunscorch Expanse — Desert
4. Frostveil Range — Snow
5. Abyssal Reach — Ocean

7 sites per map = 35 combat sites.

## Ocean

Ocean sites can be:
- above water
- floating/ship-based
- reef/platform combat
- underwater
- submerged ruins
- transition sites

## Air

Air is supported by the type contract for future expansion, but the launch configuration remains exactly five parent maps as requested.

## CS rules generated automatically

- 5v5
- first to 5 round wins
- maximum 9 rounds
- elimination only
- no capture objectives
- no Siege objectives
- no mid-round respawn
- eliminated players spectate
- Adventure World equipment is used
- Adventure World power/skills/stats are used
- CS death does not permanently destroy Adventure World equipment
- server validates competitive actions and rewards

## Critical limitation

A generic Unity script cannot safely guess the private APIs of every future system. Therefore the generator uses an explicit generation contract:

`ICSWorldGenerationProvider`

A Terrain/Infrastructure/Water/etc. system should implement this interface if you want the CS generator to actively command that system during generation.

Systems that don't implement the contract can still be connected for runtime integration, but the generator cannot invent their private method names or safely call unknown APIs.

This makes the architecture expandable without hard-coding one specific future implementation.

## Final intended flow

Lobby
→ CS selected
→ CS matchmaking
→ server match instance
→ generated/loaded CS parent map + site
→ generated environment/infrastructure
→ 5v5 elimination
→ score/result/reward
→ return to lobby
