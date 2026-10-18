using UnityEngine;

namespace SoulboundLegends.CS
{
    [CreateAssetMenu(fileName = "CSRulebook", menuName = "Soulbound Legends/CS/Rulebook")]
    public class CSRulebook : ScriptableObject
    {
        [TextArea(3,8)]
        public string purpose = "CS = Practice & Sharpen. Pure team elimination.";
        public int playersPerTeam = 5;
        public int roundsToWin = 5;
        public int maximumRounds = 9;
        public float roundDurationSeconds = 300f;

        [Header("Hard Rules")]
        public bool eliminationOnly = true;
        public bool captureObjectives = false;
        public bool siegeObjectives = false;
        public bool midRoundRespawn = false;
        public bool useAdventureWorldEquipment = true;
        public bool useAdventureWorldPower = true;
        public bool permanentEquipmentLoss = false;

        [TextArea(3,8)]
        public string eliminationRule = "A round is won only when every opposing player has been eliminated.";
        [TextArea(3,8)]
        public string timeRule = "When the timer reaches zero, the match enters pressure/sudden-death handling; elimination remains the deciding objective.";
        [TextArea(3,8)]
        public string fairnessRule = "Matchmaking and team balancing should consider player level, equipment strength, skills and battle power.";
    }
}
