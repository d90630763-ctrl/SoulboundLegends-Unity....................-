# Soulbound Legends — Rebuild Delivery Notes

This rebuild extends the previous Unity 6000.5.6f1 builder archive. Run **Soulbound Legends → Game Builder → BUILD GAME** after importing so the editor regenerates the scenes with the latest builder code.

## Added in this rebuild
- Adventure World director attached to the generated Village scene.
- Runtime resource-node generation for herbs, wood and ore; gathering checks proximity and adds items to the local inventory.
- Day/night lighting cycle and rotating local weather states (clear, cloudy, rain and arcane mist).
- Local monster pursuit/attack behavior for generated village beasts, arena enemies and Supreme Ruin creatures.
- Defeat rewards and a local respawn behavior for the player actor.
- A saved beginner quest chain: speak to guild, gather resources, defeat beasts, then claim a gated reward.
- Quest HUD and quest panel update from live local objective progress.
- Nearby interaction button opens the relevant guild, alchemy or blacksmith panel when close enough; otherwise it attempts nearby gathering.
- Healing potion use, local save button and periodic local save of player/inventory data.
- Builder validation checks for the newly attached Adventure World systems.

## Important limits — do not mistake blockouts for final art
This is a stronger playable local foundation, not a verified commercial MMORPG release. Procedural characters, weapons, buildings and resource nodes are still generated from simple meshes; they are not premium authored/rigged production assets. The local AI and quest chain are starter implementations. Full animation sets, advanced navigation, deep authored NPC dialogue, complete CS/BR/Siege rules and maps, a server-authoritative economy, anti-cheat, deployed multiplayer, authentication credentials, database hosting, Google Play Billing configuration and production QA are not completed merely by importing this archive.

## Verification boundary
- ZIP integrity and source-level consistency checks can be run outside Unity.
- Unity Editor compilation, scene regeneration, Play Mode, Android build and real-device tests have **not** been run in this environment.
- After opening, wait for import to finish, read Console errors, run **VALIDATE GENERATED PROJECT**, then **BUILD GAME** and test every generated scene/button before trusting the build.
