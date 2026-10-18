using UnityEngine;

namespace SoulboundLegends.Progression
{
    public class SBLMatchmakingBridge : MonoBehaviour
    {
        public int AdventureLevel => SBLProgressionService.Instance ? SBLProgressionService.Instance.Data.adventureLevel : 1;
        public int PlayerPower => SBLProgressionService.Instance ? SBLProgressionService.Instance.Data.totalPower : 0;
        public int MatchmakingScore => SBLProgressionService.Instance ? SBLProgressionService.Instance.GetMatchmakingScore() : 0;

        public string GetModeRank(string mode)
        {
            return SBLProgressionService.Instance ? SBLProgressionService.Instance.GetRank(mode).tier : "Bronze";
        }
    }
}
