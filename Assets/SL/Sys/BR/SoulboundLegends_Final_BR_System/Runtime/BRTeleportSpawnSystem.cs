using UnityEngine;

namespace SoulboundLegends.BR
{
    public sealed class BRTeleportSpawnSystem : MonoBehaviour
    {
        public bool serverChoosesRandomLocation = true;
        public bool showArrivalBeam = true;
        public float beamDurationSeconds = 3f;

        public void TeleportPlayerToSpawn(Transform player, Vector3 serverApprovedPosition)
        {
            if (player == null) return;
            player.position = serverApprovedPosition;
            // Connect the project's VFX/animation system here for the actual beam effect.
            // The BR rule is that nearby players can see the arrival beam.
        }
    }
}
