using System;
using System.Collections.Generic;

namespace SoulboundLegends.InstanceCoordinator
{
    [Serializable] public sealed class InstanceRequest
    {
        public string RequestId;
        public string InstanceType;
        public string WorldRegion;
        public string PlayerId;
        public List<string> PartyPlayerIds = new();
        public int MaximumCapacity = 1;
        public int MinimumCapacity = 1;
        public List<string> RequiredSystems = new();
        public string GameplayMode;
        public string Ruleset;
        public string MapEnvironment;
        public List<string> ServerRequirements = new();
        public InstanceRequestPriority Priority = InstanceRequestPriority.Normal;
        public int DurationSeconds;
        public bool PersistenceRequired;
        public bool PartyMustRemainTogether = true;
        public bool AllowPartialAssignment;
        public string PreferredServerId;
        public string PreferredInstanceId;
        public string SourceInstanceId;
    }

    [Serializable] public sealed class InstanceRecord
    {
        public string InstanceId;
        public string InstanceType;
        public string WorldRegion;
        public string ServerId;
        public int MaximumCapacity;
        public int ReservedSlots;
        public List<string> PlayerIds = new();
        public List<string> PartyIds = new();
        public InstanceLifecycleState State;
        public InstanceFailureState FailureState;
        public string FailureReason;
        public string GameplayMode;
        public string Ruleset;
        public string MapEnvironment;
        public DateTimeOffset CreatedAt;
        public DateTimeOffset LastActivityAt;
        public DateTimeOffset? ExpiresAt;
        public bool PersistenceRequired;
        public string PersistentWorldRelationship;
        public long Version;
    }

    [Serializable] public sealed class ServerSnapshot
    {
        public string ServerId;
        public string Region;
        public ServerHealth Health;
        public int MaxPlayers;
        public int CurrentPlayers;
        public int MaxInstances;
        public int CurrentInstances;
        public double CpuLoad;
        public double MemoryLoad;
        public double NetworkLoad;
        public DateTimeOffset LastHeartbeat;
        public List<string> Capabilities = new();
    }

    [Serializable] public sealed class Reservation
    {
        public string ReservationId;
        public string InstanceId;
        public string RequestId;
        public int Slots;
        public List<string> PlayerIds = new();
        public DateTimeOffset CreatedAt;
        public DateTimeOffset ExpiresAt;
        public bool Committed;
    }

    [Serializable] public sealed class TransferRecord
    {
        public string TransferId;
        public string PlayerId;
        public string SourceInstanceId;
        public string DestinationInstanceId;
        public string DestinationServerId;
        public TransferState State;
        public DateTimeOffset CreatedAt;
        public DateTimeOffset UpdatedAt;
        public string FailureReason;
    }
}
