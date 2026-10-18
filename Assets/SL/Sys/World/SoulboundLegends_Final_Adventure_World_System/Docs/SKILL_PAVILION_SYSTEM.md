# SL — Final Adventure World Skill Pavilion

## Purpose
The Adventure World Skill Pavilion is the normal-to-mid grade skill marketplace. Skill definitions are created by the General Skill System. Skill animation references can be supplied by the Human/Monster Generation/animation provider.

## Skill placement rules
- Normal-to-mid grade skills: eligible for the Skill Pavilion.
- High-grade skills: normally discovered in ruins, events, hidden world locations, or by defeating high-rank monsters.
- Collaboration/season skills: may be sold in the Pavilion even when their tier is high, when explicitly marked Collaboration/Season Only.
- Every skill has a permanent Skill ID and remains in the developer registry.

## Inspector workflow
1. Open AdventureWorldSystem.
2. Find **SKILL PAVILION + WORLD SKILL REGISTRY**.
3. Press **+ ADD SKILL**.
4. Fill in the skill ID/name/tier/rarity and connect the General Skill definition.
5. Choose **RUINS** or **PAVILION** using the placement buttons.
6. If Pavilion is selected, set the Aether price.
7. Optional: mark High-Rank Monster Drop, Event Skill, or Collaboration/Season Only.
8. Use **SYNC SKILLS FROM GENERAL SKILL + ANIMATION SYSTEMS** after connected systems are ready.
9. Use **VALIDATE SKILL REGISTRY** before generating/updating content.

## Server authority
Pavilion purchases are validated by the Server Authorization provider before ownership is assigned. The system records a transaction ID, skill ID, player ID, Aether price, completion status, server validation status, and UTC timestamp.

## Update safety
Do not regenerate the whole world to add a new skill. Add a new registry entry with a new Skill ID. Existing skills, discovery state, ownership, and transactions remain unchanged.

## Important architecture boundary
Adventure World decides where a skill appears. The General Skill System remains responsible for the actual skill definition/behavior, while compatible animation providers supply animation references. A generic Unity script cannot safely call unknown private APIs from unrelated systems; those systems must expose the provider interfaces or adapters included in this package.
