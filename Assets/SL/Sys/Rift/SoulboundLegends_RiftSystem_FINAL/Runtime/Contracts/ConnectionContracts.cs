using System;
using System.Collections.Generic;

namespace SoulboundLegends.RiftSystem
{
    public enum RiftCapability
    {
        AdventureWorld,
        WorldEvents,
        WorldKingdom,
        QuestGuild,
        MonsterGeneration,
        GeneralMonster,
        Combat,
        GeneralSkill,
        SkillVfx,
        Character,
        DynamicInstance,
        Destruction,
        Portal,
        Navigation,
        Npc,
        Inventory,
        Equipment,
        Rewards,
        Progression,
        Persistence,
        Database,
        ServerAuthority,
        ServerAuthorization,
        AntiCheat,
        Analytics,
        LoreDiscovery,
        Notifications,
        Audio,
        Ui,
        LiveOps,
        WorldState
    }

    public interface IRiftSystemProvider
    {
        string ProviderId { get; }
        int Priority { get; }
        IReadOnlyCollection<RiftCapability> Capabilities { get; }
        bool IsAvailable { get; }
    }

    public interface IRiftAdventureWorldProvider : IRiftSystemProvider
    {
        bool TryGetValidCity(string preferredCityId, out RiftCityContext city);
        bool IsWorldConditionSatisfied(RiftWorldCondition condition);
        void NotifyRiftWorldStateChanged(RiftStateSnapshot snapshot);
    }

    public interface IRiftWorldEventsProvider : IRiftSystemProvider
    {
        void Publish(RiftServerEvent evt);
    }

    public interface IRiftKingdomProvider : IRiftSystemProvider
    {
        bool TryGetCity(string cityId, out RiftCityContext city);
        bool TrySetCityEngulfed(string cityId, string riftId, long timestamp);
        bool TrySetCityDestroyed(string cityId, string riftId, long timestamp, string cause);
        bool TryClearCityEngulfment(string cityId, string riftId);
    }

    public interface IRiftQuestProvider : IRiftSystemProvider
    {
        bool CreateOrUpdateRiftQuest(RiftStateSnapshot snapshot);
        void CompleteRiftQuest(string riftId);
        void FailRiftQuest(string riftId, RiftFailureReason reason);
    }

    public interface IRiftMonsterProvider : IRiftSystemProvider
    {
        bool SpawnRiftMonster(RiftMonsterSpawnRequest request, out string serverSpawnId);
        bool DespawnRiftMonster(string serverSpawnId);
        bool TryGetMonsterState(string serverSpawnId, out RiftMonsterServerState state);
    }

    public interface IRiftCombatProvider : IRiftSystemProvider
    {
        bool ValidateBossDefeat(string riftId, string bossSpawnId, string actorPlayerId, long serverTimestamp);
    }

    public interface IRiftSkillVfxProvider : IRiftSystemProvider
    {
        void PlayRiftVfx(RiftVfxRequest request);
    }

    public interface IRiftCharacterProvider : IRiftSystemProvider
    {
        bool TryGetPlayerPosition(string playerId, out RiftVector3 position);
        bool TrySetPlayerLocked(string playerId, bool locked, string reason);
        bool TryTeleportPlayer(string playerId, RiftVector3 position, string reason);
        bool TryGetAlive(string playerId, out bool alive);
    }

    public interface IRiftInstanceProvider : IRiftSystemProvider
    {
        bool TryCreateInstance(RiftInstanceRequest request, out RiftInstanceResult result);
        bool TryDestroyInstance(string instanceId, string reason);
        bool TryMovePlayerToInstance(string playerId, string instanceId);
        bool TryRemovePlayerFromInstance(string playerId, string instanceId);
        bool TryGetInstancePlayerCount(string instanceId, out int count);
    }

    public interface IRiftDestructionProvider : IRiftSystemProvider
    {
        /// <summary>
        /// Returns true only when the connected world/destruction system has authoritatively
        /// determined that the city's destruction condition has been met. This call must not
        /// grant destruction merely because it was queried.
        /// </summary>
        bool EvaluateWorldDestruction(RiftDestructionRequest request);
    }

    public interface IRiftPortalProvider : IRiftSystemProvider
    {
        bool CanUsePortal(string playerId, string portalId, string destinationId);
        void BlockDestination(string destinationId, string reason);
    }

    public interface IRiftNavigationProvider : IRiftSystemProvider
    {
        void SetAreaRestricted(string areaId, bool restricted, string reason);
    }

    public interface IRiftNpcProvider : IRiftSystemProvider
    {
        void SetAreaAccessRestricted(string areaId, bool restricted, string reason);
        void NotifyCityEngulfed(string cityId, string riftId);
        void NotifyCityDestroyed(string cityId, string riftId);
    }

    public interface IRiftRewardProvider : IRiftSystemProvider
    {
        bool TryGrantRewards(RiftRewardGrantRequest request);
    }

    public interface IRiftProgressionProvider : IRiftSystemProvider
    {
        void HandleRiftFailurePunishment(RiftPunishmentContext context);
    }

    public interface IRiftPersistenceProvider : IRiftSystemProvider
    {
        bool TryLoad(string riftId, out RiftStateSnapshot snapshot);
        bool TrySave(RiftStateSnapshot snapshot, string expectedVersionToken, out string newVersionToken);
        bool TryDelete(string riftId);
        IEnumerable<RiftStateSnapshot> LoadActiveRifts();
    }

    public interface IRiftDatabaseProvider : IRiftPersistenceProvider { }

    public interface IRiftServerAuthorizationProvider : IRiftSystemProvider
    {
        bool Authorize(RiftAuthorizationRequest request, out string denialReason);
    }

    public interface IRiftServerAuthorityProvider : IRiftSystemProvider
    {
        bool IsAuthoritativeServer { get; }
        long ServerUnixMilliseconds { get; }
        bool ValidateRequestNonce(string playerId, string nonce);
        bool ConsumeRateLimit(string playerId, string action, int cost);
    }

    public interface IRiftAntiCheatProvider : IRiftSystemProvider
    {
        void Report(RiftAnomalyReport report);
    }

    public interface IRiftAnalyticsProvider : IRiftSystemProvider
    {
        void Record(RiftServerEvent evt);
    }

    public interface IRiftLoreProvider : IRiftSystemProvider
    {
        void RecordDiscovery(RiftDiscoveryEvent discovery);
    }

    public interface IRiftNotificationProvider : IRiftSystemProvider
    {
        void Notify(RiftNotification notification);
    }

    public interface IRiftAudioProvider : IRiftSystemProvider
    {
        void Play(RiftAudioRequest request);
    }

    public interface IRiftUiProvider : IRiftSystemProvider
    {
        void PushSnapshot(RiftStateSnapshot snapshot);
    }

    public interface IRiftLiveOpsProvider : IRiftSystemProvider
    {
        bool AuthorizeAdminCommand(RiftAdminCommand command);
    }

    [Serializable]
    public struct RiftVector3
    {
        public float X;
        public float Y;
        public float Z;
    }

    [Serializable]
    public sealed class RiftCityContext
    {
        public string CityId;
        public string KingdomId;
        public RiftVector3 Center;
        public float Radius;
        public bool IsValid;
    }

    [Serializable]
    public sealed class RiftWorldCondition
    {
        public string ConditionId;
        public int MinimumWorldLevel;
        public bool RequireCityActive = true;
    }

    [Serializable]
    public sealed class RiftMonsterSpawnRequest
    {
        public string RiftId;
        public string SpawnPointId;
        public string MonsterDefinitionId;
        public int Level;
        public bool IsElite;
        public bool IsBoss;
        public int Wave;
    }

    [Serializable]
    public sealed class RiftMonsterServerState
    {
        public string SpawnId;
        public bool IsAlive;
        public string RiftId;
    }

    [Serializable]
    public sealed class RiftInstanceRequest
    {
        public string RiftId;
        public string CityId;
        public int Capacity;
        public bool PartyAllowed;
        public bool ServerAuthoritative = true;
    }

    [Serializable]
    public sealed class RiftInstanceResult
    {
        public string InstanceId;
        public bool Created;
        public int Capacity;
    }

    [Serializable]
    public sealed class RiftDestructionRequest
    {
        public string RiftId;
        public string CityId;
        public string KingdomId;
        public long Timestamp;
        public string Cause;
        public IReadOnlyList<string> AffectedPlayerIds;
    }

    [Serializable]
    public sealed class RiftRewardGrantRequest
    {
        public string RiftId;
        public string PlayerId;
        public IReadOnlyList<RiftRewardRecord> Rewards;
        public string IdempotencyKey;
    }

    [Serializable]
    public sealed class RiftPunishmentContext
    {
        public string PlayerId;
        public string RiftId;
        public string CityId;
        public string FailureReason;
        public long DestructionTimestamp;
    }

    [Serializable]
    public sealed class RiftAuthorizationRequest
    {
        public string PlayerId;
        public string Action;
        public string RiftId;
        public string CityId;
        public string Nonce;
        public long ClientTimestamp;
        public long ServerTimestamp;
    }

    [Serializable]
    public sealed class RiftAnomalyReport
    {
        public string PlayerId;
        public string RiftId;
        public string AnomalyType;
        public string Details;
        public long Timestamp;
        public int Severity;
    }

    [Serializable]
    public sealed class RiftVfxRequest
    {
        public string RiftId;
        public string EffectId;
        public RiftVector3 Position;
        public float Scale;
    }

    [Serializable]
    public sealed class RiftNotification
    {
        public string NotificationKey;
        public string RiftId;
        public string CityId;
        public string KingdomId;
        public string Message;
        public int Severity;
    }

    [Serializable]
    public sealed class RiftAudioRequest
    {
        public string RiftId;
        public string AudioKey;
        public RiftVector3 Position;
    }
}
