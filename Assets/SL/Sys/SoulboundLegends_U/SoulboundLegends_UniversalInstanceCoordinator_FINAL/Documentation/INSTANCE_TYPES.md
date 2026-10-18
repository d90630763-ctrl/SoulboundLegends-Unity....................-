# Instance type integration

No finite enum of gameplay types is used. `InstanceType` is a string and server adapters are registered by that type. This supports current and future types such as Adventure World, Village, Kingdom, City, Inn, Shop, Tavern, Interior, Dungeon, Boss Arena, Event, Rift, Supreme Ruin, CS, Battle Royale, Siege and Training without changing coordinator code.

Recommended adapter ownership:
- Adventure World/Village/Kingdom/City: Adventure World shard system
- Inn/Shop/Tavern/Interior: Infrastructure/interior system
- Dungeon/Boss/Event/Training: Dungeon/Boss/Event system
- Rift: Rift system
- Supreme Ruin: Supreme Ruin system
- CS: CS system
- Battle Royale: BR system
- Siege: Siege system
