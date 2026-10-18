# SL — FINAL ADVENTURE WORLD SYSTEM

This package is the master Adventure World architecture for Village, Town, City and Kingdom worlds.

## Permanent progression rule
Adventure World is the source of permanent weapons, skills, powers, equipment, materials and progression used by Clash/CS, Battle Royale and Siege. Those modes do not create duplicate permanent progression.

## World types
- Village
- Town
- City
- Kingdom

## Core systems
Terrain, Fantasy Infrastructure, NPC, Monster/Human Generation, General Monster AI, General Monster Skill, Portal, Crafting, Materials/Plants, Server Authorization, Networking, Player/Character, Combat, Skills, Equipment, Inventory, Stats/Battle Power, Animation, Housing, Guild, Party/Friends, Auction House, Cloud/Weather, Water, Destruction, Quest, Rewards, Rank, Game State, Time Sync, Localization, Analytics, Reporting and Scene Loading connect through provider slots.

## Skill Pavilion
The package includes AdventureWorldSkillSystem. It provides:
- Skill registry with permanent IDs
- Skill tier and rarity tracking
- Pavilion placement and Aether pricing
- Ruin/world discovery placement
- High-rank monster drop placement
- Event placement
- Collaboration/season Pavilion exceptions
- General Skill System synchronization
- Animation provider synchronization
- Developer-only registry tracking
- Server-authorized Pavilion purchase transactions
- Update-safe append-only skill registration

## Player entry
Lobby -> server validation -> world load -> saved customized player character -> spawn -> Player Script enabled -> General HUD -> gameplay.

## Instances
Village capacity defaults to 35 players. Village assignment persists through logout and reconnect until the player leaves the Village tier for Town/City/Kingdom progression.

## Update safety
Generate New Content Only is intended for post-release additions. Existing region IDs, artifact IDs, material IDs, skill IDs, ownership, discovery state and transaction history are preserved by the registry architecture.

## Provider requirement
Connected external systems must implement the compatible provider interfaces/adapters. The package cannot invoke arbitrary private APIs from unrelated Unity scripts.
