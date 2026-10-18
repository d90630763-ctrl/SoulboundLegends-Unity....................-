using UnityEngine;

namespace SoulboundLegends.Progression
{
    public class SBLClassEvaluator
    {
        public string Evaluate(SBLStats s, out float confidence)
        {
            float warrior = s.attack * 1.0f + s.defense * .65f + s.stamina * .25f;
            float tank = s.defense * 1.25f + s.hp * .12f + s.magicDefense * .45f;
            float mage = s.magicAttack * 1.2f + s.mp * .10f + s.skillPower * 1.2f;
            float ranger = s.attack * .75f + s.critical * 1.25f + s.attackSpeed * 1.15f + s.movementSpeed * .35f;
            float assassin = s.attack * .9f + s.critical * 1.4f + s.attackSpeed * 1.35f + s.movementSpeed * .6f;
            float support = s.magicDefense * .55f + s.mp * .08f + s.skillPower * 1.45f;
            float spellblade = s.attack * .75f + s.magicAttack * .75f + s.skillPower * .9f + s.attackSpeed * .5f;

            string name = "Novice";
            float best = 0f;
            float second = 0f;

            float[] values = { warrior, tank, mage, ranger, assassin, support, spellblade };
            string[] names = { "Warrior", "Guardian", "Mage", "Ranger", "Assassin", "Support", "Spellblade" };

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > best) { second = best; best = values[i]; name = names[i]; }
                else if (values[i] > second) second = values[i];
            }

            confidence = best <= 0 ? 0 : Mathf.Clamp01((best - second) / best) * 100f;
            return name;
        }
    }
}
