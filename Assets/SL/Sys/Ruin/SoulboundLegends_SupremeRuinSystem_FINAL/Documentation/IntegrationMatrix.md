# Integration Matrix

| Capability | Provider contract | Used by |
|---|---|---|
| Requirements | ISupremeRuinRequirements | Item/level/quest/story/world/God/discovery validation and atomic item consumption |\n| Infrastructure | ISupremeRuinInfrastructure | Build/destroy Ruin environment |
| Quest | ISupremeRuinQuest | Divine Trial and five kill objectives |
| Monster Generation | ISupremeRuinMonster | Five Divine Creatures |
| Combat | ISupremeRuinCombat | Authoritative creature death proof |
| Character | ISupremeRuinCharacter | Eligibility, alive state, instance membership |
| Dynamic Instance | ISupremeRuinDynamicInstance | Trial instance lifecycle |
| World State | ISupremeRuinWorld | Availability, occupancy, release |
| Server Authority | ISupremeRuinServerAuthority | Time, nonce, security |
| Server Authorization | ISupremeRuinAuthorization | Player/admin authorization |
| Requirements | ISupremeRuinRequirements | Requirements and atomic item transaction |
| Persistence | ISupremeRuinPersistence | Ruin/trial snapshots |
| Inheritance | ISupremeRuinInheritance | Permanent inheritance ownership |
| Rewards | ISupremeRuinRewards | Optional configured rewards |
| Anti-Cheat | ISupremeRuinAntiCheat | Suspicious behavior |
| Analytics | ISupremeRuinAnalytics | Server events |
| Lore/Discovery | ISupremeRuinLore | Progressive discovery |
| UI | ISupremeRuinUi | Server-authoritative display |
| Live Ops | ISupremeRuinLiveOps | Authorized administration |

## Generic connections

`SupremeRuinConnectionHub` is the only connection surface. Provider components declare capabilities and are discovered automatically.

## No duplicated responsibilities

This package never generates ordinary terrain, ordinary monster assets, networking, database storage, inventory, combat damage, or UI gameplay authority. Those responsibilities stay with connected SL systems.
