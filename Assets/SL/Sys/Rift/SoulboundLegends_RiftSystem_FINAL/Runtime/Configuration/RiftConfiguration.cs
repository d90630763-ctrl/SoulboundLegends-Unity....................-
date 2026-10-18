using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.RiftSystem
{
    [CreateAssetMenu(menuName = "Soulbound Legends/Rift/Rift Configuration", fileName = "RiftConfiguration")]
    public sealed class RiftConfiguration : ScriptableObject
    {
        [Header("Spawn")]
        [Min(1)] public int MinimumSecondsBetweenRifts = 1800;
        [Min(1)] public int MaximumSecondsBetweenRifts = 7200;
        public List<RiftWorldCondition> WorldConditions = new();
        public List<string> AllowedKingdomIds = new();

        [Header("Instance")]
        [Min(1)] public int DefaultInstanceCapacity = 20;
        public bool AllowParties = true;
        [Min(1)] public int MaxConcurrentInstances = 100;

        [Header("Rift")]
        public List<RiftTypeDefinition> RiftTypes = new();
        public List<RiftDifficultyDefinition> Difficulties = new();
        [Min(1)] public int DefaultAttemptLimit = 3;
        [Min(1)] public int DefaultDurationSeconds = 900;

        [Header("Persistence")]
        [Min(1)] public int StateSchemaVersion = 1;
        [Min(1)] public int AutosaveSeconds = 5;

        [Header("Security")]
        [Min(1)] public int RequestRateLimitCost = 1;
        [Min(0)] public int PositionValidationToleranceMeters = 8;
        [Min(0)] public int TimerDriftToleranceSeconds = 3;

        [Header("Mobile")]
        [Min(1)] public int MaxWorldInvasionMonsters = 80;
        [Min(1)] public int MaxBossesPerRift = 1;
        [Min(1)] public int NetworkSnapshotIntervalMilliseconds = 250;
        [Min(1)] public int VfxPoolPrewarm = 16;

        private void OnValidate()
        {
            if (MaximumSecondsBetweenRifts < MinimumSecondsBetweenRifts)
                MaximumSecondsBetweenRifts = MinimumSecondsBetweenRifts;
            if (DefaultInstanceCapacity < 1)
                DefaultInstanceCapacity = 1;
        }
    }

    [Serializable]
    public sealed class RiftTypeDefinition
    {
        public string Id = "rift_default";
        public string DisplayName = "Unknown Rift";
        public int Weight = 1;
        public List<string> EnvironmentIds = new();
        public List<string> MonsterDefinitionIds = new();
        public List<string> BossDefinitionIds = new();
        public List<string> HazardIds = new();
        public List<RiftSpawnWaveDefinition> Waves = new();
        public List<RiftRewardDefinition> Rewards = new();
        public int AttemptLimit = 3;
        public int DurationSeconds = 900;
        public int InstanceCapacity = 20;
        public bool AllowParties = true;
    }

    [Serializable]
    public sealed class RiftDifficultyDefinition
    {
        public string Id = "unknown";
        public int Weight = 1;
        public int MonsterLevelOffset;
        public int BossLevelOffset;
        public float RewardMultiplier = 1f;
    }

    [Serializable]
    public sealed class RiftSpawnWaveDefinition
    {
        public int Wave = 1;
        public int DelaySeconds = 5;
        public int Population = 5;
        [Min(1)] public int MonsterBaseLevel = 1;
        [Min(1)] public int BossBaseLevel = 1;
        public bool IsBossWave;
        public List<string> MonsterPool = new();
        public List<string> SpawnPointIds = new();
    }

    [Serializable]
    public sealed class RiftRewardDefinition
    {
        public string RewardId;
        public string RewardType;
        public string ItemId;
        public long Amount;
        [Range(0f, 100f)] public float ChancePercent = 100f;
    }
}
