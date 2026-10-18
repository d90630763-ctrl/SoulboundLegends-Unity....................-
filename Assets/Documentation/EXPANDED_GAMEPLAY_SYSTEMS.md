# Soulbound Legends — Expanded Gameplay Foundation

This package adds Builder-generated local gameplay foundations and UI actions for the Adventure World village. Use **Soulbound Legends → Game Builder → BUILD GAME** to regenerate scenes after importing the project.

## Added in this revision
- Village gameplay inventory/resource store with starter materials.
- Starter weapon generator creates sword, staff and bow blockout objects on the player weapon socket; UI commands can equip them.
- Blacksmith panel with recipe text and buttons to forge Iron Sword, Apprentice Staff and Hunter Bow.
- Alchemy panel with a Minor Healing Potion recipe and Brew button.
- Gather button adds herbs, wood and ore to inventory.
- Village NPCs receive roles/greetings: Adventure Guild Guide, Alchemist, Blacksmith and resident.
- Guild quest dialogue/status and a one-time local test reward.
- Inventory refresh and visible on-screen action feedback.
- Generated UI buttons are wired to named gameplay commands; recipe ingredient checks happen before materials are consumed.

## Recipes in this local foundation
- Minor Healing Potion: 2 Herbs + 1 Water Flask.
- Iron Sword: 3 Iron Ore + 1 Wood.
- Apprentice Staff: 3 Wood + 1 Monster Core.
- Hunter Bow: 3 Wood + 1 Herb.

## Important limitations (do not mistake a foundation for a finished live MMO)
- Weapon meshes are primitive blockout geometry, not final art-quality weapons. Bowstring, weapon-specific attack animations, projectile simulation, balanced damage and full equipment-stat effects require further implementation and testing.
- Alchemy currently makes an inventory item; healing consumption/effect and a complete potion progression tree still need implementation.
- NPC dialogue and quest progression here are local test foundations. Production dialogue trees, branching objectives, quest persistence, rewards and NPC navigation/voice need full content and integration.
- Inventory/progression are local test data. Multiplayer crafting, player persistence, trading, drops and anti-duplication must be enforced by a deployed authoritative server before release.
- The builder can create procedural blockout scenes, primitive geometry and UI; it cannot automatically supply a finished AAA art library, professional animations, voice acting, production backend credentials, or prove multiplayer/network operation.
- This package has not been compiled in Unity Editor or runtime-tested by this archive creation step. Open the project, allow Unity to import, inspect Console, run Builder validation, then test every button in Play mode. Resolve all errors before Android build.
