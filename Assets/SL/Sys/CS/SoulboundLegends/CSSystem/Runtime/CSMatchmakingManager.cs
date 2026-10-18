using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.CS
{
    public class CSMatchmakingManager : MonoBehaviour
    {
        [Header("Fairness")]
        public float acceptableBattlePowerDifference = 0.20f;
        public float acceptableLevelDifference = 10f;
        public bool prioritizePartyIntegrity = true;
        public bool balanceTeamBattlePower = true;

        public List<CSPlayerSlot> BuildCandidateList(List<CSPlayerSlot> queue, float targetPower)
        {
            var result = new List<CSPlayerSlot>();
            foreach (var p in queue)
            {
                if (targetPower <= 0 || Mathf.Abs(p.battlePower - targetPower) / Mathf.Max(1f, targetPower) <= acceptableBattlePowerDifference)
                    result.Add(p);
            }
            return result;
        }
    }
}
