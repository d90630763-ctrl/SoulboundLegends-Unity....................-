using System;
using System.Collections.Generic;

namespace SoulboundLegends.RiftSystem
{
    [Serializable]
    public sealed class RiftParticipantRecord
    {
        public string PlayerId;
        public RiftParticipantStatus Status;
        public long EnteredUnixMilliseconds;
        public long LastSeenUnixMilliseconds;
        public bool WasInsideAtEngulfment;
        public bool WasInsideAtDestruction;
        public int DeathCount;
    }

    [Serializable]
    public sealed class RiftMonsterRecord
    {
        public string SpawnId;
        public string MonsterDefinitionId;
        public int Level;
        public bool IsElite;
        public bool IsBoss;
        public bool IsAlive;
        public int SpawnWave;
        public string TargetId;
    }

    [Serializable]
    public sealed class RiftRewardRecord
    {
        public string RewardId;
        public string RewardType;
        public string ItemId;
        public long Amount;
        public bool Granted;
    }

    [Serializable]
    public sealed class RiftStateSnapshot
    {
        public int SchemaVersion = 1;
        public string RiftId;
        public string RiftTypeId;
        public string RiftDifficultyId;
        public string KingdomId;
        public string CityId;
        public RiftState State;
        public RiftFailureReason FailureReason;
        public long SpawnUnixMilliseconds;
        public long ExpirationUnixMilliseconds;
        public long StateChangedUnixMilliseconds;
        public int AttemptLimit;
        public int AttemptsConsumed;
        public string CurrentInstanceId;
        public RiftWorldConsequence WorldConsequence;
        public bool CompletionRecorded;
        public bool RewardsRecorded;
        public bool DestructionRecorded;
        public string BossDefinitionId;
        public bool BossSpawned;
        public bool BossDefeated;
        public int CurrentWave;
        public long NextWaveUnixMilliseconds;
        public List<int> SpawnedWaveNumbers = new();
        public List<RiftParticipantRecord> Participants = new();
        public List<RiftMonsterRecord> Monsters = new();
        public List<RiftRewardRecord> Rewards = new();
        public string VersionToken;
    }

    [Serializable]
    public sealed class RiftDiscoveryEvent
    {
        public string RiftId;
        public RiftDiscoveryType DiscoveryType;
        public string DiscoveryKey;
        public string PlayerId;
        public string EvidenceId;
        public long TimestampUnixMilliseconds;
    }

    [Serializable]
    public sealed class RiftAdminCommand
    {
        public string CommandId;
        public string AdministratorId;
        public string RiftId;
        public string RiftTypeId;
        public string CityId;
        public int? DurationSeconds;
        public int? AttemptLimit;
        public string BossDefinitionId;
        public string Reason;
        public long IssuedUnixMilliseconds;
    }

    [Serializable]
    public sealed class RiftServerEvent
    {
        public string EventId;
        public string RiftId;
        public string PlayerId;
        public string CityId;
        public string KingdomId;
        public string EventType;
        public string FailureReason;
        public long TimestampUnixMilliseconds;
        public string PayloadJson;
    }
}
