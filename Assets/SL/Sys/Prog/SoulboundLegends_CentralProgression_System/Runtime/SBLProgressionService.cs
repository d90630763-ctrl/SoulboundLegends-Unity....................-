using System;
using UnityEngine;

namespace SoulboundLegends.Progression
{
    [DefaultExecutionOrder(-500)]
    public class SBLProgressionService : MonoBehaviour
    {
        public static SBLProgressionService Instance { get; private set; }

        [SerializeField] private SBLProgressionConfig config;
        [SerializeField] private SBLPlayerProgressionData data = new SBLPlayerProgressionData();

        public SBLPlayerProgressionData Data => data;
        public SBLProgressionConfig Config => config;

        public event Action OnProgressionChanged;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (config == null)
            {
                config = ScriptableObject.CreateInstance<SBLProgressionConfig>();
            }
            Recalculate();
        }

        public void AddAdventureXP(int amount)
        {
            if (amount <= 0) return;
            data.adventureXP += amount;

            while (data.adventureLevel < data.adventureLevelCap &&
                   data.adventureXP >= config.GetXPForNextLevel(data.adventureLevel))
            {
                data.adventureXP -= config.GetXPForNextLevel(data.adventureLevel);
                data.adventureLevel++;
            }

            Recalculate();
        }

        public void SetEquipmentStats(SBLStats stats)
        {
            data.equipmentStats = stats;
            Recalculate();
        }

        public void SetBonusStats(SBLStats stats)
        {
            data.bonusStats = stats;
            Recalculate();
        }

        public void RecordCompetitiveMatch(string mode, bool won)
        {
            SBLRankData rank = GetRank(mode);
            rank.rating = Mathf.Max(0, rank.rating + (won ? config.ratingPerWin : -config.ratingPerLoss));
            if (won) rank.wins++; else rank.losses++;
            rank.tier = CalculateTier(rank.rating);
            OnProgressionChanged?.Invoke();
        }

        public SBLRankData GetRank(string mode)
        {
            if (string.Equals(mode, "CS", StringComparison.OrdinalIgnoreCase)) return data.csRank;
            if (string.Equals(mode, "BR", StringComparison.OrdinalIgnoreCase)) return data.brRank;
            if (string.Equals(mode, "Siege", StringComparison.OrdinalIgnoreCase)) return data.siegeRank;
            return data.csRank;
        }

        public string CalculateTier(int rating)
        {
            if (rating >= 3000) return "Mythic";
            if (rating >= 2400) return "Diamond";
            if (rating >= 1800) return "Platinum";
            if (rating >= 1200) return "Gold";
            if (rating >= 600) return "Silver";
            return "Bronze";
        }

        public void ExpandAdventureCap()
        {
            int next = Mathf.Min(config.maximumCap, Mathf.Max(data.adventureLevelCap + 100, 100));
            data.adventureLevelCap = next;
            OnProgressionChanged?.Invoke();
        }

        public int GetMatchmakingScore()
        {
            return data.adventureLevel * config.adventureLevelWeight +
                   data.totalPower * config.powerWeight;
        }

        public void Recalculate()
        {
            int lv = Mathf.Max(1, data.adventureLevel);
            data.levelStats = new SBLStats
            {
                hp = lv * config.hpPerLevel,
                mp = lv * config.mpPerLevel,
                attack = lv * config.attackPerLevel,
                defense = lv * config.defensePerLevel,
                magicAttack = lv * config.magicAttackPerLevel,
                magicDefense = lv * config.magicDefensePerLevel,
                attackSpeed = lv * config.attackSpeedPerLevel,
                movementSpeed = lv * config.movementSpeedPerLevel,
                critical = lv * config.criticalPerLevel,
                stamina = lv * config.staminaPerLevel,
                skillPower = lv * config.skillPowerPerLevel
            };

            data.totalStats = data.levelStats + data.equipmentStats + data.bonusStats;
            data.totalPower = data.totalStats.CalculatePower();

            SBLClassEvaluator evaluator = new SBLClassEvaluator();
            data.evaluatedClass = evaluator.Evaluate(data.totalStats, out data.classConfidence);

            data.csRank.tier = CalculateTier(data.csRank.rating);
            data.brRank.tier = CalculateTier(data.brRank.rating);
            data.siegeRank.tier = CalculateTier(data.siegeRank.rating);

            OnProgressionChanged?.Invoke();
        }
    }
}
