using UnityEngine;

namespace SoulboundLegends.BR
{
    public sealed class BRGameBootstrap : MonoBehaviour
    {
        public BRWorldGenerator worldGenerator;
        public BRMatchManager matchManager;
        public BRTeleportSpawnSystem teleportSystem;
        public BRSafeZoneSystem safeZoneSystem;

        public void StartBR()
        {
            if (matchManager != null) matchManager.BeginTeleportation();
        }
    }
}
