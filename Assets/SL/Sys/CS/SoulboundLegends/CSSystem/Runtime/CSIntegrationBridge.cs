using UnityEngine;

namespace SoulboundLegends.CS
{
    public class CSIntegrationBridge : MonoBehaviour
    {
        [Header("Shared Systems")]
        public bool useSharedGeneralHUD = true;
        public bool useAdventureWorldEquipment = true;
        public bool useAdventureWorldPower = true;
        public bool returnToLobbyAfterMatch = true;

        public void PushHUDState(int round, int scoreA, int scoreB, float timeRemaining, int aliveA, int aliveB)
        {
            // Integration hook for the existing General HUD.
        }

        public void RequestReturnToLobby()
        {
            // Integration hook for the existing Lobby/Scene Loading system.
        }
    }
}
