using UnityEngine;
namespace GameSystems.AdvancedLiquidWater {
    public enum LiquidType { Water, River, Lake, Sea, Ocean, Swamp, Lava, Poison, Custom }
    [CreateAssetMenu(fileName="LiquidProfile",menuName="Game Systems/Liquid/Liquid Profile")]
    public sealed class LiquidProfile:ScriptableObject {
        public LiquidType type; public Material material; public Color color=Color.white;
        public float density=1, flowSpeed=1, waveHeight=.1f, damagePerSecond=0;
        public bool buoyant=true, terrainAwareFlow=true, useHybridSimulation=true;
    }
    public sealed class LiquidBody:MonoBehaviour {
        public LiquidProfile profile; public float width=10,length=10,waterLevel=0,depth=2;
        public bool customShape, linkToTerrainChannels;
    }
}