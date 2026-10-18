using UnityEngine;
namespace GameSystems.AdvancedTerrainGenerator {
    [CreateAssetMenu(fileName="TerrainDescription",menuName="Game Systems/Advanced Terrain/Terrain Description")]
    public sealed class TerrainDescription : ScriptableObject {
        [TextArea(3,10)] public string description;
        [Min(16)] public int resolution=129; public int seed=12345; public float width=500,height=120,length=500;
        [Range(0,1)] public float roughness=.55f; [Range(0,1)] public float erosion=.35f;
        public int channelCount=3; public bool allowDryChannels=true;
    }
    public sealed class TerrainChannelPoint { public Vector3 position; public float width; public float depth; public int branch; }
}