using System;
using System.Collections.Generic;

namespace SoulboundLegends.SupremeRuin
{
    [Serializable]
    public struct RuinVector3
    {
        public float X, Y, Z;
        public RuinVector3(float x, float y, float z) { X=x; Y=y; Z=z; }
    }

    [Serializable]
    public sealed class RuinParticipant
    {
        public string PlayerId;
        public string CharacterId;
        public long JoinedUnixMilliseconds;
        public bool IsInside;
        public bool CanAssist;
    }

    [Serializable]
    public sealed class CreatureProgressRecord
    {
        public string CreatureId;
        public string SpawnId;
        public string DefinitionId;
        public int RequiredIndex;
        public bool Spawned;
        public bool Defeated;
        public long DefeatedAtUnixMilliseconds;
        public string DefeatedByPlayerId;
    }

    [Serializable]
    public sealed class SupremeRuinStateSnapshot
    {
        public int SchemaVersion = 1;
        public string TrialId;
        public string RuinId;
        public string GodId;
        public string KingdomId;
        public string EntranceId;
        public string InstanceId;
        public RuinState RuinState;
        public TrialState TrialState;
        public string ActiveChallengerPlayerId;
        public long StartedUnixMilliseconds;
        public long LastUpdatedUnixMilliseconds;
        public long TrialDeadlineUnixMilliseconds;
        public int RequiredCreatureCount;
        public int DefeatedCreatureCount;
        public List<CreatureProgressRecord> Creatures = new();
        public List<RuinParticipant> Participants = new();
        public InheritanceAwardState InheritanceAwardState;
        public string AwardedInheritanceId;
        public TrialFailureReason FailureReason;
        public string FailureDetail;
        public long Version;
    }

    [Serializable]
    public sealed class RuinAccessRequest
    {
        public string RequestId;
        public string PlayerId;
        public string CharacterId;
        public string RuinId;
        public string EntranceId;
        public string Nonce;
        public long ClientTimestamp;
    }

    [Serializable]
    public sealed class RuinAccessDecision
    {
        public bool Allowed;
        public string ReasonCode;
        public string ReasonMessage;
        public string TrialId;
        public string InstanceId;
    }

    [Serializable]
    public sealed class CreatureDeathValidationRequest
    {
        public string RequestId;
        public string TrialId;
        public string SpawnId;
        public string CreatureDefinitionId;
        public string KillerPlayerId;
        public long ServerTimestamp;
        public string CombatProof;
    }

    [Serializable]
    public sealed class InheritanceRecord
    {
        public int SchemaVersion = 1;
        public string PlayerId;
        public string CharacterId;
        public string GodId;
        public string InheritanceId;
        public string TrialId;
        public string RuinId;
        public long AcquisitionUnixMilliseconds;
        public long Version;
    }

    [Serializable]
    public sealed class RuinRewardRequest
    {
        public string TrialId;
        public string PlayerId;
        public string InheritanceId;
        public long ServerTimestamp;
    }

    [Serializable]
    public sealed class RuinAdminCommand
    {
        public string CommandId;
        public string AdministratorId;
        public string RuinId;
        public string PlayerId;
        public string Reason;
        public long IssuedUnixMilliseconds;
    }
}
