using System.Collections.Generic;

namespace SoulboundLegends.SupremeRuin
{
    public static class SupremeRuinRuntimeValidator
    {
        public static bool ValidateConfiguration(SupremeRuinConfiguration configuration, out List<string> errors)
        {
            errors = new List<string>();
            if (configuration == null) { errors.Add("Configuration is null."); return false; }

            foreach (var ruin in configuration.Ruins)
            {
                if (ruin == null) { errors.Add("Null Ruin definition."); continue; }
                if (ruin.RequiredCreatures == null || ruin.RequiredCreatures.Count != 5)
                    errors.Add($"Ruin {ruin.Id} must have exactly five Divine Creatures.");

                var god = configuration.FindGod(ruin.GodId);
                if (god == null) { errors.Add($"Ruin {ruin.Id} has missing God {ruin.GodId}."); continue; }

                var seen = new HashSet<int>();
                foreach (var slot in ruin.RequiredCreatures)
                {
                    if (!seen.Add(slot.RequiredIndex))
                        errors.Add($"Ruin {ruin.Id} has duplicate creature index {slot.RequiredIndex}.");

                    var creature = configuration.FindCreature(slot.CreatureDefinitionId);
                    if (creature == null) { errors.Add($"Missing creature {slot.CreatureDefinitionId}."); continue; }

                    bool elementOk = god.God == SupremeGodId.Omnigod
                        ? god.OmnigodAllowedElements.Contains(creature.Element) || creature.Element == DivineElement.MultiElement
                        : creature.Element.ToString() == god.Element.ToString();

                    if (god.God != SupremeGodId.Shadow && creature.God != god.God)
                        errors.Add($"Creature {creature.Id} has wrong God association for Ruin {ruin.Id}.");

                    if (!elementOk)
                        errors.Add($"Creature {creature.Id} has invalid element for God {god.Id}.");
                }
            }
            return errors.Count == 0;
        }
    }
}
