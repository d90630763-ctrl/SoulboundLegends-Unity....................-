using UnityEngine;

namespace SoulboundLegends.AdventureWorld
{
    public class AdventureWorldPlacementSystem : MonoBehaviour
    {
        public bool BlendConnectedEdges(WorldRegionData a, WorldRegionData b)
        {
            if (a == null || b == null || a.placementMode != WorldPlacementMode.ConnectedFixed || b.placementMode != WorldPlacementMode.ConnectedFixed) return false;
            return true;
        }
        public Vector3 DirectionOffset(WorldDirection direction, float distance)
        {
            return direction switch { WorldDirection.North => Vector3.forward * distance, WorldDirection.South => Vector3.back * distance, WorldDirection.East => Vector3.right * distance, _ => Vector3.left * distance };
        }
    }
}
