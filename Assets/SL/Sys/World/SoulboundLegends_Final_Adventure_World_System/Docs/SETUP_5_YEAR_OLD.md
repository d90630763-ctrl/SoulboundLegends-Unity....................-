# Setup — very simple steps

1. Make a new Unity project using Unity 2022.3 LTS or a compatible newer Unity version that supports this package's C# syntax.
2. Make an empty GameObject. Name it `Adventure World System`.
3. Add `AdventureWorldSystem`.
4. Add `AdventureWorldGenerator`.
5. Add `AdventureWorldArtifactSystem`.
6. Add `AdventureWorldMaterialSystem`.
7. Add `AdventureWorldAuctionHouseSystem`.
8. Add `AdventureWorldContentRegistry`.
9. Add `AdventureWorldBootstrap`.
10. In the AdventureWorldSystem Inspector, choose Village, Town, City or Kingdom.
11. Press `AUTO-CREATE ALL CONNECTION SLOTS`.
12. Drag your existing Terrain System into the Terrain connection slot.
13. Drag your Fantasy Infrastructure System into the infrastructure slot.
14. Drag your General NPC System into the NPC slot.
15. Drag your Monster/Human Generation System into the monster-generation slot.
16. Connect General Monster AI and General Monster Skill systems.
17. Connect your Player Script, saved Player Character, Character Customization, Character Controller, Camera and General HUD.
18. Connect Server Authorization and Networking.
19. Connect Crafting and Material/Plant systems for artifact/material generation.
20. Connect Cloud/Weather for dynamic weather.
21. Connect Housing, Guild and Party systems when those systems are available.
22. Press `VALIDATE ADVENTURE WORLD`.
23. Press `GENERATE SELECTED WORLD` for the initial world.
24. For a later update, use `GENERATE NEW CONTENT ONLY` so old content is preserved.
25. Do NOT connect a Village to a City automatically. Connect the Portal System only when you want a real portal transition.
26. Test player entry: Lobby -> server authorization -> world loading -> saved customized player -> spawn -> General HUD -> gameplay.

## Important
This ZIP is the final Adventure World architecture and Unity runtime/editor package. It does not secretly contain your other systems' private implementations. Those systems must be connected through their provider interface or an adapter.
