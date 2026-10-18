using System;
using UnityEngine;

namespace SoulboundLegends.Progression
{
    [Serializable]
    public struct SBLStats
    {
        public int hp, mp, attack, defense, magicAttack, magicDefense;
        public int attackSpeed, movementSpeed, critical, stamina, skillPower;
        public int power;

        public static SBLStats operator +(SBLStats a, SBLStats b)
        {
            return new SBLStats
            {
                hp = a.hp + b.hp, mp = a.mp + b.mp,
                attack = a.attack + b.attack, defense = a.defense + b.defense,
                magicAttack = a.magicAttack + b.magicAttack,
                magicDefense = a.magicDefense + b.magicDefense,
                attackSpeed = a.attackSpeed + b.attackSpeed,
                movementSpeed = a.movementSpeed + b.movementSpeed,
                critical = a.critical + b.critical, stamina = a.stamina + b.stamina,
                skillPower = a.skillPower + b.skillPower
            };
        }

        public int CalculatePower()
        {
            return Mathf.Max(0,
                hp / 10 + mp / 10 + attack + defense + magicAttack +
                magicDefense + attackSpeed + movementSpeed + critical +
                stamina + skillPower);
        }
    }

    [Serializable]
    public class SBLRankData
    {
        public string tier = "Bronze";
        public int rating = 0;
        public int wins = 0;
        public int losses = 0;
    }

    [Serializable]
    public class SBLPlayerProgressionData
    {
        public int adventureLevel = 1;
        public int adventureXP = 0;
        public int adventureLevelCap = 100;

        public SBLRankData csRank = new SBLRankData();
        public SBLRankData brRank = new SBLRankData();
        public SBLRankData siegeRank = new SBLRankData();

        public SBLStats levelStats;
        public SBLStats equipmentStats;
        public SBLStats bonusStats;
        public SBLStats totalStats;

        public string evaluatedClass = "Novice";
        public float classConfidence = 0f;
        public int totalPower = 0;
    }
}
