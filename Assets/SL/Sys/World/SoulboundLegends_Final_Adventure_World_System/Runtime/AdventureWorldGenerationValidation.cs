using System.Collections.Generic;

namespace SoulboundLegends.AdventureWorld
{
    public static class AdventureWorldGenerationValidation
    {
        public static List<string> Validate(AdventureWorldSystem s)
        {
            var errors = new List<string>();
            if (s == null) { errors.Add("AdventureWorldSystem is missing."); return errors; }
            if (string.IsNullOrWhiteSpace(s.worldId)) errors.Add("World ID is required.");
            if (s.maxPlayersPerVillage < 1) errors.Add("Maximum players must be at least 1.");
            if (s.cityUnlockLevel < 1) errors.Add("City unlock level must be at least 1.");
            if (s.worldType == AdventureWorldType.Village && s.maxPlayersPerVillage != 35) errors.Add("Village default capacity is 35; change only if intentionally configured.");
            if (s.adventureWorldIsPermanentProgressionSource == false) errors.Add("Permanent progression source rule should remain enabled for Soulbound Legends.");
            var skill = s.GetComponent<AdventureWorldSkillSystem>();
            if (skill != null)
            {
                foreach (var record in skill.skills)
                {
                    if (record == null) continue;
                    if (string.IsNullOrWhiteSpace(record.skillId)) errors.Add("A skill is missing its permanent Skill ID.");
                    if (record.appearsInPavilion && record.pavilionPriceAether < 0) errors.Add("A Pavilion skill has an invalid Aether price: " + record.skillName);
                    if (record.appearsInPavilion && !record.collaborationOrSeasonOnly && record.tier > skill.maximumNormalPavilionTier) errors.Add("High-grade skill is assigned to the normal Pavilion pool: " + record.skillName);
                }
            }
            return errors;
        }
    }
}
