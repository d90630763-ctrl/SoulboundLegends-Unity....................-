# SL — Final BR System

## Purpose
Battle Royale is the game's **Adapt & Survive** mode.

## Five launch maps
1. Aetherwild Expanse — Grasslands / Forest / Highlands
2. Kingdoms of Ruin — Ruins / Kingdoms
3. Sunfire Frontier — Desert / Canyons
4. Frostbound Dominion — Snow / Mountains
5. Abyssal Realm — Ocean / Islands / Underwater

## Core BR rules
- Massive open-world match.
- Last surviving player/team wins.
- Solo, Duo and Squad are supported.
- No CS round system.
- No capture-site objective.
- No Siege objective.
- No separate BR permanent equipment/skill progression.
- Weapons, skills, powers, equipment and permanent stats come from Adventure World.
- BR death does not permanently destroy Adventure World possessions.
- Entry is anime-style server-randomized teleportation.
- Arrival is marked by a visible beam of light that nearby players can see.
- Safe zone progressively shrinks.
- Players outside the safe zone take damage.
- Team modes can use downed/revival/spectator rules.
- Optional world NPCs, monsters, bosses and dynamic events can be generated.
- Match statistics and rewards feed shared systems.
- Match ends in results and returns to Lobby.
- General HUD is shared with all game modes.

## Generation
Press **GENERATE FINAL BR WORLD** after connecting the required systems.

The generator creates the five map data structures, points of interest, teleport spawn data, BR rules and integration data. Connected systems that implement `IBRWorldGenerationProvider` can create the actual terrain, infrastructure, water, materials, weather, NPCs, monsters and other generated content.

## Provider interface
```csharp
public class MyTerrainGenerator : MonoBehaviour, IBRWorldGenerationProvider
{
    public bool CanGenerate(BRGeneratorSystemType type)
    {
        return type == BRGeneratorSystemType.TerrainGenerator;
    }

    public bool GenerateForBR(
        BRGenerationContext context,
        BRGenerationRequest request,
        out string report)
    {
        // Build the five BR worlds using context.maps.
        report = "BR terrain generated.";
        return true;
    }
}
```

The same provider pattern applies to Infrastructure, Water, Materials/Textures, Weather/Lighting, Destruction, NPCs and Monsters.
