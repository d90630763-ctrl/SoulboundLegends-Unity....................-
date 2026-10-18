SOULBOUND LEGENDS — WORLD NPC ASSEMBLY & GENERATION SYSTEM — FINAL

Copy Assets/SL/WorldNPC into your Unity project's Assets folder.

Then in Unity:
1. Let Unity compile.
2. Menu: SL > World NPC > Create Starter Databases.
3. Create an empty GameObject named World NPC Assembly.
4. Add NPCSystemRegistry.
5. Add your existing systems through Connected Systems (+ Add System).
6. Add WorldNPCAssemblyGenerator.
7. Assign your NPC prefab.
8. Assign the generated Occupation Database.
9. Use Primary NPCs for important hand-defined NPCs or Secondary NPCs for population generation.

IMPORTANT:
The package deliberately uses capability interfaces instead of guessing the private API of your existing ZIP systems. Your existing Voice, Quest, Crafting, Alchemy, Shop, Navigation, UI and Server systems can be connected with small adapters implementing the interfaces in Runtime/Interfaces.

Crafting and Alchemy are occupation-restricted. A connected Crafting/Alchemy system is NOT automatically usable by every NPC.

The included NPCServerAuthorizationAdapter is a DEVELOPMENT adapter only. For live multiplayer, connect the actual authoritative server authorization system.
