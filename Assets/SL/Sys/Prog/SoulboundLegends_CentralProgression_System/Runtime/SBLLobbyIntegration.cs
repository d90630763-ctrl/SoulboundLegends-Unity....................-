using UnityEngine;

namespace SoulboundLegends.Progression
{
    // Drop this on the Lobby/System Manager. It does not require a hard reference
    // to the Lobby implementation, preventing circular dependencies.
    public class SBLLobbyIntegration : MonoBehaviour
    {
        [SerializeField] private SBLStatsPanel lobbyStatsPanel;
        [SerializeField] private SBLStatsPanel profileStatsPanel;

        public void OpenLobbyStats() { if (lobbyStatsPanel) lobbyStatsPanel.Show(); }
        public void OpenProfileStats() { if (profileStatsPanel) profileStatsPanel.Show(); }

        public int AdventureLevel => SBLProgressionService.Instance ? SBLProgressionService.Instance.Data.adventureLevel : 1;
        public int TotalPower => SBLProgressionService.Instance ? SBLProgressionService.Instance.Data.totalPower : 0;
        public string EvaluatedClass => SBLProgressionService.Instance ? SBLProgressionService.Instance.Data.evaluatedClass : "Novice";
    }
}
