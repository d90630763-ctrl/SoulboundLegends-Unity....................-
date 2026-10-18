using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.SupremeRuin
{
    [CreateAssetMenu(menuName="Soulbound Legends/Supreme Ruin/System Configuration")]
    public sealed class SupremeRuinConfiguration : ScriptableObject
    {
        [Min(1)] public int SchemaVersion = 1;
        public List<GodDefinition> Gods = new();
        public List<DivineCreatureDefinition> DivineCreatures = new();
        public List<SupremeRuinDefinition> Ruins = new();
        public TrialGlobalRules GlobalRules = new();
        public PerformanceRules Performance = new();
        public HiddenLoreRules HiddenLore = new();

        public GodDefinition FindGod(string id) => Gods.Find(x => x.Id == id);
        public SupremeRuinDefinition FindRuin(string id) => Ruins.Find(x => x.Id == id);
        public DivineCreatureDefinition FindCreature(string id) => DivineCreatures.Find(x => x.Id == id);

        private void OnValidate()
        {
            var seen = new HashSet<string>();
            foreach (var god in Gods)
            {
                if (god == null || string.IsNullOrWhiteSpace(god.Id)) continue;
                if (!seen.Add(god.Id)) Debug.LogError($"Duplicate God ID: {god.Id}", this);
            }

            seen.Clear();
            foreach (var ruin in Ruins)
            {
                if (ruin == null || string.IsNullOrWhiteSpace(ruin.Id)) continue;
                if (!seen.Add(ruin.Id)) Debug.LogError($"Duplicate Ruin ID: {ruin.Id}", this);
                if (ruin.RequiredCreatures.Count != 5)
                    Debug.LogError($"Ruin {ruin.Id} must define exactly five Divine Creatures.", this);
            }
        }
    }

    [Serializable]
    public sealed class GodDefinition
    {
        public string Id;
        public SupremeGodId God;
        public string DisplayName;
        public DivineElement Element;
        public bool PlayerFacingEarlyLoreAllowed = true;
        public string InheritanceId;
        public List<DivineElement> OmnigodAllowedElements = new();
        public string HiddenLoreKey;
    }

    [Serializable]
    public sealed class SupremeRuinDefinition
    {
        public string Id;
        public string DisplayName;
        public string GodId;
        public string KingdomId;
        public string EntranceDefinitionId;
        public string InfrastructureDefinitionId;
        public string TrialEnvironmentDefinitionId;
        public bool AllowParty;
        [Min(1)] public int MaxPlayers = 1;
        [Min(1)] public int TrialDurationSeconds = 3600;
        public CreatureSpawnMode SpawnMode = CreatureSpawnMode.Sequential;
        public List<RuinRequirementDefinition> Requirements = new();
        public List<DivineCreatureTrialSlot> RequiredCreatures = new();
        public List<string> RewardDefinitionIds = new();
        public List<string> LoreDiscoveryIds = new();
        public FailureRules Failure = new();
        public List<string> SpawnPointIds = new();
        public string AltarDefinitionId;
        public bool RevealGodIdentityInUi = true;
    }

    [Serializable]
    public sealed class RuinRequirementDefinition
    {
        public RequirementType Type;
        public string Id;
        public int Quantity = 1;
        public int MinimumLevel;
        public string ConditionValue;
        public bool ConsumeOnSuccessfulStart = true;
    }

    [Serializable]
    public sealed class DivineCreatureTrialSlot
    {
        public string CreatureDefinitionId;
        [Min(1)] public int RequiredIndex;
        public string SpawnPointId;
        public string PhaseId;
    }

    [Serializable]
    public sealed class DivineCreatureDefinition
    {
        public string Id;
        public string SpeciesTemplateId;
        public string DisplayName;
        public SupremeGodId God;
        public DivineElement Element;
        public DivineRole Role;
        public int BaseLevel = 100;
        public DivineCreatureStatProfile Stats = new();
        public List<string> SkillDefinitionIds = new();
        public List<string> ResistanceIds = new();
        public List<string> WeaknessIds = new();
        public List<string> AiProfileIds = new();
        public List<string> PhaseDefinitionIds = new();
        public List<string> VfxDefinitionIds = new();
        public List<string> RewardDefinitionIds = new();
    }

    [Serializable]
    public sealed class DivineCreatureStatProfile
    {
        [Min(1)] public double Hp = 1000000000d;
        [Min(1)] public double Armor = 10000000d;
        [Min(1)] public double Attack = 10000000d;
        [Min(0.01f)] public double AttackSpeed = 2d;
        [Min(0.01f)] public double MovementSpeed = 8d;
        [Min(0)] public double Resistance = 0.8d;
        [Min(1)] public double ElementalPower = 10000000d;
        [Min(1)] public double CriticalPower = 10d;
        [Min(1)] public double SkillDamageMultiplier = 25d;
        [Min(0)] public double HealingPerSecond = 100000d;
        [Min(0)] public double Regeneration = 100000d;
        [Range(0,1)] public float StatusResistance = 0.95f;
        [Range(0,1)] public float CrowdControlResistance = 0.98f;
        [Min(0.01f)] public double AiReactionSeconds = 0.05d;
        [Min(1)] public double PhaseMultiplier = 5d;
    }

    [Serializable]
    public sealed class TrialGlobalRules
    {
        public bool ExactlyFiveCreaturesRequired = true;
        public bool PreventInheritanceOwnersFromAnyTrial = true;
        public bool PreventInheritanceOwnersFromAssisting = true;
        public bool OneActiveChallengerPerRuin = true;
        public bool RequireServerAuthority = true;
        public bool RequireCombatDeathProof = true;
        public bool RequireAtomicInheritanceAward = true;
        [Min(1)] public int DisconnectGraceSeconds = 120;
        [Min(0)] public int RequestRateLimitPerMinute = 20;
    }

    [Serializable]
    public sealed class FailureRules
    {
        public bool FailOnPlayerDeath = true;
        public bool FailOnAbandon = true;
        public bool FailOnDisconnectTimeout = true;
        public bool FailOnServerTermination = true;
        public bool FailOnInvalidState = true;
    }

    [Serializable]
    public sealed class PerformanceRules
    {
        [Min(1)] public int MaxNetworkUpdatesPerSecond = 10;
        [Min(1)] public int MaxUiUpdatesPerSecond = 5;
        [Min(1)] public int MaxConcurrentRuinInstances = 100;
        public bool EnablePoolingHints = true;
        public bool AllowReducedVfxQuality = true;
    }

    [Serializable]
    public sealed class HiddenLoreRules
    {
        public string DemonWorldTruthKey = "ruin.demonworld.truth";
        public bool HideDemonWorldTruthEarly = true;
        public bool HideShadowBetrayalEarly = true;
        public List<string> EarlyHiddenKeys = new();
    }
}
