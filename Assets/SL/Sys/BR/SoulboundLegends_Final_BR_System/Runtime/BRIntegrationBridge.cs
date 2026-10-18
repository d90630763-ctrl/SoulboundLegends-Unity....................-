using UnityEngine;

namespace SoulboundLegends.BR
{
    public sealed class BRIntegrationBridge : MonoBehaviour
    {
        [Header("Shared Systems")]
        public MonoBehaviour lobbySystem;
        public MonoBehaviour generalHUDSystem;
        public MonoBehaviour characterSystem;
        public MonoBehaviour combatSystem;
        public MonoBehaviour skillsSystem;
        public MonoBehaviour equipmentSystem;
        public MonoBehaviour serverAuthoritySystem;
        public MonoBehaviour networkingSystem;
        public MonoBehaviour rewardsProgressionSystem;

        public void EnterBRFromLobby()
        {
            // Scene/match loading is intentionally delegated to the connected Scene/World Loading system.
        }

        public void ReturnToLobby()
        {
            // Results/rewards are processed first; the connected Lobby/Scene system performs the actual return.
        }
    }
}
