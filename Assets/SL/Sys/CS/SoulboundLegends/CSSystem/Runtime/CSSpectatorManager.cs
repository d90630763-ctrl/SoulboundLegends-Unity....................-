using UnityEngine;

namespace SoulboundLegends.CS
{
    public class CSSpectatorManager : MonoBehaviour
    {
        public bool allowSpectatingAliveTeammates = true;
        public bool allowFreeCamera = false;

        public void OnPlayerEliminated(string playerId)
        {
            // Integration hook for the shared Camera/HUD system.
        }
    }
}
