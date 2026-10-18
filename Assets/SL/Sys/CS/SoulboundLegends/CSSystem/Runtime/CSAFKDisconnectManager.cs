using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.CS
{
    public class CSAFKDisconnectManager : MonoBehaviour
    {
        public float afkWarningSeconds = 60f;
        public float afkPenaltySeconds = 120f;
        public bool allowReconnect = true;
        public bool penalizeVoluntaryAbandonment = true;

        private readonly Dictionary<string, float> lastActivity = new Dictionary<string, float>();

        public void MarkActivity(string playerId)
        {
            if (string.IsNullOrEmpty(playerId)) return;
            lastActivity[playerId] = Time.time;
        }

        public bool IsAFK(string playerId)
        {
            if (!lastActivity.ContainsKey(playerId)) return false;
            return Time.time - lastActivity[playerId] >= afkPenaltySeconds;
        }
    }
}
