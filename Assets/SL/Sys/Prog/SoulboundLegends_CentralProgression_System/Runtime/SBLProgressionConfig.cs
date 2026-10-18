using UnityEngine;

namespace SoulboundLegends.Progression
{
    [CreateAssetMenu(menuName = "Soulbound Legends/Progression Config", fileName = "SBL_ProgressionConfig")]
    public class SBLProgressionConfig : ScriptableObject
    {
        [Header("Adventure Level")]
        public int startingLevel = 1;
        public int startingCap = 100;
        public int maximumCap = 1000;
        public int xpBase = 100;
        public float xpGrowth = 1.12f;

        [Header("Guaranteed Stats Per Level")]
        public int hpPerLevel = 100;
        public int mpPerLevel = 50;
        public int attackPerLevel = 10;
        public int defensePerLevel = 10;
        public int magicAttackPerLevel = 10;
        public int magicDefensePerLevel = 10;
        public int attackSpeedPerLevel = 1;
        public int movementSpeedPerLevel = 1;
        public int criticalPerLevel = 1;
        public int staminaPerLevel = 5;
        public int skillPowerPerLevel = 2;

        [Header("Competitive Rank")]
        public int startingRating = 0;
        public int ratingPerWin = 25;
        public int ratingPerLoss = 15;
        public int ratingPerDraw = 0;

        [Header("Matchmaking")]
        public int adventureLevelWeight = 60;
        public int powerWeight = 40;

        public int GetXPForNextLevel(int level)
        {
            return Mathf.Max(1, Mathf.RoundToInt(xpBase * Mathf.Pow(xpGrowth, Mathf.Max(0, level - 1))));
        }
    }
}
