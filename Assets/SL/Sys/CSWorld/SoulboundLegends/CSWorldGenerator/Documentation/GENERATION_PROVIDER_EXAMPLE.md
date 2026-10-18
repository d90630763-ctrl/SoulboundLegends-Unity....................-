# How a Terrain/Infrastructure system becomes an active generator

A shared generator should implement:

public class MyTerrainGenerator : MonoBehaviour, ICSWorldGenerationProvider
{
    public bool CanGenerate(CSGeneratorSystemType type)
    {
        return type == CSGeneratorSystemType.TerrainGenerator;
    }

    public bool GenerateForCS(
        CSGenerationContext context,
        CSGenerationRequest request,
        out string report)
    {
        // Read context.maps.
        // For each map/site, use your own terrain-generation implementation.
        // Use the parentProperties and environmentInstruction values.
        // Build/assign terrain objects under context.worldRoot.
        report = "Terrain generated for CS.";
        return true;
    }
}

The same pattern can be used for:
- Infrastructure
- Water
- Materials/textures
- Weather/lighting
- Destruction
- NPCs/monsters
- Character model generation

This avoids the CS generator pretending to know the internal API of systems that may be changed later.
