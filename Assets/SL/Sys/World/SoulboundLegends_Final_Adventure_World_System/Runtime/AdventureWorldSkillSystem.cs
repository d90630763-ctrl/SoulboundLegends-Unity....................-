using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.AdventureWorld
{
    public enum AdventureSkillTier { Tier1, Tier2, Tier3, Tier4, Tier5, Tier6, Tier7 }
    public enum AdventureSkillRarity { Common, Uncommon, Rare, Epic, Legendary, Mythic, Divine, GodTier }
    public enum AdventureSkillSource { SkillPavilion, Ruin, HighRankMonster, Event, Collaboration, Season, HiddenWorldDiscovery }
    public enum SkillPavilionAvailability { NotAvailable, Pavilion }

    [Serializable]
    public class AdventureSkillRecord
    {
        [Tooltip("Permanent unique ID. Never reuse an old ID.")]
        public string skillId;
        public string skillName = "New Skill";
        [TextArea(2, 5)] public string description;
        public AdventureSkillTier tier = AdventureSkillTier.Tier1;
        public AdventureSkillRarity rarity = AdventureSkillRarity.Common;

        [Header("SOURCE / PLACEMENT")]
        public AdventureSkillSource primarySource = AdventureSkillSource.SkillPavilion;
        public bool appearsInRuin;
        public bool appearsInPavilion;
        public bool appearsAsHighRankMonsterDrop;
        public bool appearsInEvents;
        public bool collaborationOrSeasonOnly;
        public string regionId;
        public string locationId;
        public string requiredMonsterId;

        [Header("PAVILION")]
        public long pavilionPriceAether;
        public bool pavilionPriceSet;
        public bool pavilionEnabled = true;
        public string pavilionCategory = "General Skills";

        [Header("SKILL SYSTEM ASSETS")]
        public string generalSkillDefinitionId;
        public string animationProviderId;
        public string vfxProviderId;
        public bool definitionVerified;
        public bool animationVerified;
        public bool serverVerified;

        [Header("TRACKING")]
        public string generationVersion = "1.0.0";
        public bool discovered;
        public bool claimed;
        public string ownerId;
        public string notes;
    }

    public interface IAdventureWorldGeneralSkillProvider
    {
        bool CanProvideSkills { get; }
        IEnumerable<AdventureSkillRecord> GetSkillDefinitions();
        bool ValidateSkillDefinition(string skillId);
    }

    public interface IAdventureWorldSkillAnimationProvider
    {
        bool CanProvideSkillAnimations { get; }
        string GetSkillAnimationProviderId(string skillId);
        bool HasAnimationForSkill(string skillId);
    }

    [Serializable]
    public class SkillPavilionTransaction
    {
        public string transactionId;
        public string skillId;
        public string playerId;
        public long priceAether;
        public string status;
        public bool serverValidated;
        public string timestamp;
    }

    public class AdventureWorldSkillSystem : MonoBehaviour
    {
        [Header("SKILL PAVILION")]
        public bool enableSkillPavilion = true;
        [Tooltip("Pavilion is intended for normal-to-mid grade skills. High grade skills should normally be world discoveries unless marked Collaboration/Season.")]
        public AdventureSkillTier maximumNormalPavilionTier = AdventureSkillTier.Tier3;
        public bool allowCollaborationAndSeasonSkillsInPavilion = true;
        public bool serverAuthoritativeTransactions = true;

        [Header("SKILL REGISTRY — DEVELOPER TRACKING")]
        [Tooltip("Use + Add Skill in the custom Inspector. Existing records are preserved on updates.")]
        public List<AdventureSkillRecord> skills = new();
        public List<SkillPavilionTransaction> transactions = new();
        public string registryVersion = "1.0.0";

        [Header("CONNECTIONS")]
        public List<AdventureWorldSystemSlot> skillSystemConnections = new();

        public void AddSkill(AdventureSkillRecord skill)
        {
            if (skill == null) return;
            if (string.IsNullOrWhiteSpace(skill.skillId)) skill.skillId = CreateStableId(skill.skillName);
            if (skills.Exists(x => x.skillId == skill.skillId)) return;
            skills.Add(skill);
        }

        public bool SetRuinPlacement(string skillId, bool enabled)
        {
            var s = Find(skillId); if (s == null) return false;
            s.appearsInRuin = enabled;
            if (enabled) s.primarySource = AdventureSkillSource.Ruin;
            return true;
        }

        public bool SetPavilionPlacement(string skillId, bool enabled, long priceAether)
        {
            var s = Find(skillId); if (s == null) return false;
            s.appearsInPavilion = enabled;
            s.pavilionPriceAether = Math.Max(0, priceAether);
            s.pavilionPriceSet = enabled;
            if (enabled) s.primarySource = AdventureSkillSource.SkillPavilion;
            return true;
        }

        public AdventureSkillRecord Find(string skillId) => skills.Find(x => x != null && x.skillId == skillId);

        public bool CanAppearInPavilion(AdventureSkillRecord skill)
        {
            if (skill == null || !enableSkillPavilion || !skill.appearsInPavilion || !skill.pavilionEnabled) return false;
            if (skill.collaborationOrSeasonOnly) return allowCollaborationAndSeasonSkillsInPavilion;
            return skill.tier <= maximumNormalPavilionTier;
        }

        public bool PurchaseSkill(string playerId, string skillId)
        {
            var skill = Find(skillId);
            if (!CanAppearInPavilion(skill) || !skill.pavilionPriceSet) return false;
            if (serverAuthoritativeTransactions && !ServerAllows("SkillPavilionPurchase", playerId, skillId + "|" + skill.pavilionPriceAether)) return false;
            if (skill.claimed && !string.IsNullOrEmpty(skill.ownerId)) return false;

            skill.claimed = true;
            skill.ownerId = playerId;
            skill.serverVerified = true;
            transactions.Add(new SkillPavilionTransaction
            {
                transactionId = "skill_tx_" + Guid.NewGuid().ToString("N"),
                skillId = skillId,
                playerId = playerId,
                priceAether = skill.pavilionPriceAether,
                status = "Completed",
                serverValidated = true,
                timestamp = DateTime.UtcNow.ToString("O")
            });
            return true;
        }

        public void SyncFromProviders()
        {
            foreach (var slot in skillSystemConnections)
            {
                if (slot == null || slot.connectedSystem == null) continue;
                if (slot.connectedSystem is IAdventureWorldGeneralSkillProvider provider && provider.CanProvideSkills)
                {
                    foreach (var incoming in provider.GetSkillDefinitions())
                    {
                        if (incoming == null) continue;
                        var existing = Find(incoming.skillId);
                        if (existing == null) AddSkill(incoming);
                        else existing.definitionVerified = provider.ValidateSkillDefinition(existing.skillId);
                    }
                }

                if (slot.connectedSystem is IAdventureWorldSkillAnimationProvider animationProvider && animationProvider.CanProvideSkillAnimations)
                {
                    foreach (var skill in skills)
                    {
                        if (skill == null || string.IsNullOrEmpty(skill.skillId)) continue;
                        if (animationProvider.HasAnimationForSkill(skill.skillId))
                        {
                            skill.animationProviderId = animationProvider.GetSkillAnimationProviderId(skill.skillId);
                            skill.animationVerified = true;
                        }
                    }
                }
            }
        }

        public List<AdventureSkillRecord> GetPavilionSkills()
        {
            var result = new List<AdventureSkillRecord>();
            foreach (var skill in skills) if (CanAppearInPavilion(skill)) result.Add(skill);
            return result;
        }

        public List<AdventureSkillRecord> GetRuinSkills()
        {
            var result = new List<AdventureSkillRecord>();
            foreach (var skill in skills) if (skill != null && skill.appearsInRuin) result.Add(skill);
            return result;
        }

        bool ServerAllows(string action, string playerId, string payload)
        {
            foreach (var slot in skillSystemConnections)
            {
                if (slot?.connectedSystem is IAdventureWorldServerAuthorizationProvider server)
                    return server.Validate(action, playerId, payload);
            }
            var bridge = GetComponent<AdventureWorldServerAuthorizationBridge>();
            return bridge != null && bridge.ValidateAction(action, playerId, payload);
        }

        static string CreateStableId(string name)
        {
            string clean = string.IsNullOrWhiteSpace(name) ? "skill" : name.Trim().ToLowerInvariant().Replace(" ", "_");
            return "skill_" + clean + "_" + Guid.NewGuid().ToString("N").Substring(0, 8);
        }
    }
}
