# SL — World NPC Assembly & Generation System — FINAL

## Purpose
A general-purpose NPC assembly/orchestration system for the entire SL world. It is not limited to a village.

## What it contains
- Primary and Secondary NPC generation.
- Automatic mixed NPC gender generation. There is deliberately no Male/Female generator button.
- Individual NPC identity, occupation, personality, age, faction, importance and story role.
- Expandable `[+ Add System]` registry instead of a fixed list of system slots.
- Capability detection through `INPCSystemCapabilityProvider`.
- Future-system compatibility through interfaces/capability discovery.
- Occupation database and editable occupation rules.
- Population generator and NPC templates/build requests.
- Navigation data: boundaries, home, workplace, allowed/forbidden areas, destinations and patrol routes.
- Routine data for wake/work/eat/rest/talk/wander/patrol/travel/sleep/emergency/combat/return-home.
- Mandatory approach greeting path for every NPC.
- Dialogue database support with occupation-based fallback greetings.
- Voice-system adapter contract. Generated gender selects the voice category; the connected Voice System supplies an individual voice profile.
- Crafting and Alchemy occupation gating.
- Quest, Shop, UI, Navigation, Inventory, Equipment, Guild, Reputation, Bounty, Party, Teleport, Combat and Server Authorization capability slots through the generic registry.
- Editable content database for potions, weapons, armor, materials, recipes, shop items, NPC services and regions.
- Server authorization hooks for interaction/action validation.
- Editor buttons for generating primary/secondary NPCs and rebuilding capability detection.

## Important integration rule
The package cannot safely guess the private APIs of your other custom ZIP systems. Instead it provides stable interfaces. Your existing systems should implement the appropriate interface (or receive a tiny adapter) and then be placed in the Registry's `[+ Add System]` list. The registry discovers capabilities automatically.

## Crafting / Alchemy rule
Connecting a Crafting System or Alchemy System does NOT make every NPC a crafter/alchemist. The occupation definition and provider must approve the occupation. Example default adapter IDs:
- Crafting: blacksmith, guild_blacksmith, carpenter, artisan
- Alchemy: alchemist, guild_alchemist, healer

Replace/extend these occupation IDs in your own occupation database.

## Voice rule
NPC visual gender is generated automatically. The generator has no Male/Female selection button. The generated gender is passed to the Voice System, which selects from that gender's voice pool. Individual voice profiles can differ by pitch, speed, tone and whatever the connected Voice System supports.

## Server authority
`NPCServerAuthorizationAdapter` is only a local development adapter. For the live multiplayer game, connect the actual authoritative server validation system through the generic registry. Do not trust the client for gold, inventory, item creation, quest rewards, crafting, alchemy, purchases, XP, rank, permissions or other authoritative state.

## Basic setup
1. Import the folder into the Unity project's `Assets` folder.
2. Create an empty GameObject named `World NPC Assembly`.
3. Add `NPCSystemRegistry`.
4. Add any compatible system components using `Connected Systems (+ Add System)`.
5. Add `WorldNPCAssemblyGenerator`.
6. Assign an NPC prefab containing `NPCController` (and optionally `NPCInteractionController`, `NPCServiceRouter`, NavMeshAgent, Animator, etc.).
7. Create an `NPCOccupationDatabase` and add occupation definitions.
8. Add population entries for Secondary NPC generation or build requests for Primary NPCs.
9. Connect the actual Voice System. Its adapter must implement `INPCVoiceProvider`.
10. Connect the actual Quest/Crafting/Alchemy/Shop/UI/Navigation/Server systems using their capability interfaces or adapters.
11. Generate Primary or Secondary NPCs from the generator Inspector.

## External system contract
Implement:
- `INPCSystemCapabilityProvider` on every system you want the NPC registry to discover.
- Add the specific functional interface for the capability you want the NPC system to call.

Available functional contracts are in `Runtime/Interfaces/INPCSystemInterfaces.cs`.

## No hidden hard-coded village limit
The same assembly system can be placed in a village, city, kingdom, guild, palace, outpost or future region. Region-specific behavior comes from data, occupation rules, templates and connected systems.
