using System;
using System.Collections.Generic;

namespace SoulboundLegends.SupremeRuin
{
    public enum SupremeRuinCapability
    {
        Infrastructure, Requirements, Quest, MonsterGeneration, GeneralMonster, Combat, Skills, SkillVfx,
        Character, DynamicInstance, Destruction, ServerAuthority, ServerAuthorization,
        Persistence, Database, Inventory, Equipment, Progression, Inheritance, Rewards, WorldState,
        AdventureWorld, WorldKingdom, Npc, Ui, Audio, Analytics, AntiCheat, LoreDiscovery, LiveOps
    }

    public interface ISupremeRuinSystemProvider
    {
        string ProviderId { get; }
        bool IsAvailable { get; }
        IEnumerable<SupremeRuinCapability> Capabilities { get; }
    }

    public interface ISupremeRuinServerAuthority : ISupremeRuinSystemProvider
    {
        bool IsAuthoritativeServer { get; }
        long ServerNowUnixMilliseconds { get; }
        bool ValidateRequestNonce(string playerId, string nonce, string requestId);
        void ReportSecurity(string playerId, string ruinId, string code, string detail, int severity);
    }

    public interface ISupremeRuinAuthorization : ISupremeRuinSystemProvider
    {
        bool Authorize(string playerId, string action, string resourceId);
        bool AuthorizeAdmin(string administratorId, string action, string resourceId);
    }

    public interface ISupremeRuinPersistence : ISupremeRuinSystemProvider
    {
        bool Save(SupremeRuinStateSnapshot snapshot);
        bool Load(string ruinId, out SupremeRuinStateSnapshot snapshot);
        bool SaveInheritance(InheritanceRecord record);
        bool LoadInheritance(string playerId, out InheritanceRecord record);
        bool SaveDestroyedTrialMarker(string trialId);
    }

    public interface ISupremeRuinDatabase : ISupremeRuinSystemProvider { string SchemaNamespace { get; } }

    public interface ISupremeRuinInfrastructure : ISupremeRuinSystemProvider
    {
        bool BuildRuin(string ruinId, string definitionId, string instanceId);
        bool DestroyRuin(string ruinId, string instanceId);
    }

    public interface ISupremeRuinDynamicInstance : ISupremeRuinSystemProvider
    {
        bool CreateTrialInstance(string requestId, string ruinId, string ownerPlayerId, string definitionId, out string instanceId);
        bool AddPlayer(string instanceId, string playerId);
        bool RemovePlayer(string instanceId, string playerId);
        bool DestroyInstance(string instanceId, string reason);
    }

    public interface ISupremeRuinRequirements : ISupremeRuinSystemProvider
    {
        bool CheckItem(string playerId, string itemId, int quantity);
        bool CheckLevel(string playerId, int minimumLevel);
        bool CheckQuest(string playerId, string questId);
        bool CheckStory(string playerId, string storyFlag);
        bool CheckWorld(string worldCondition);
        bool CheckGodCondition(string playerId, string conditionId);
        bool CheckPermission(string playerId, string permissionId);
        bool CheckDiscovery(string playerId, string discoveryId);
        bool ConsumeItemsAtomically(string playerId, IReadOnlyList<ItemRequirementData> requirements);
    }

    public interface ISupremeRuinQuest : ISupremeRuinSystemProvider
    {
        bool CreateDivineTrialQuest(string playerId, string trialId, string ruinId, string godId, int requiredCreatures);
        bool UpdateCreatureDefeat(string trialId, int creatureIndex);
        bool CompleteTrialQuest(string trialId);
        bool FailTrialQuest(string trialId, TrialFailureReason reason);
    }

    public interface ISupremeRuinMonster : ISupremeRuinSystemProvider
    {
        bool SpawnDivineCreature(DivineCreatureSpawnRequest request, out string spawnId);
        bool Despawn(string spawnId);
    }

    public interface ISupremeRuinCombat : ISupremeRuinSystemProvider
    {
        bool ValidateCreatureDeath(CreatureDeathValidationRequest request, out CombatDeathProof proof);
    }

    public interface ISupremeRuinCharacter : ISupremeRuinSystemProvider
    {
        bool IsCharacterEligibleForTrial(string playerId);
        bool IsPlayerAlive(string playerId);
        bool IsPlayerInInstance(string playerId, string instanceId);
    }

    public interface ISupremeRuinInheritance : ISupremeRuinSystemProvider
    {
        bool HasInheritance(string playerId);
        bool AwardInheritanceAtomically(InheritanceRecord record);
    }

    public interface ISupremeRuinRewards : ISupremeRuinSystemProvider
    {
        bool GrantConfiguredRewards(RuinRewardRequest request);
    }

    public interface ISupremeRuinWorld : ISupremeRuinSystemProvider
    {
        bool IsRuinAreaAvailable(string kingdomId, string entranceId);
        bool IsRuinOccupied(string ruinId);
        bool TryMarkRuinOccupied(string ruinId, string trialId);
        bool TryReleaseRuin(string ruinId, string trialId);
    }

    public interface ISupremeRuinLore : ISupremeRuinSystemProvider
    {
        void PublishDiscovery(string playerId, string discoveryId, string contextId);
        bool IsHiddenLoreAllowed(string loreId, string playerId);
    }

    public interface ISupremeRuinAntiCheat : ISupremeRuinSystemProvider
    {
        void Report(string playerId, string ruinId, string code, string details, int severity);
    }

    public interface ISupremeRuinAnalytics : ISupremeRuinSystemProvider
    {
        void Track(string eventName, string ruinId, string trialId, string playerId, string payload);
    }

    public interface ISupremeRuinLiveOps : ISupremeRuinSystemProvider
    {
        bool AuthorizeAdminCommand(RuinAdminCommand command);
    }

    public interface ISupremeRuinPlayerPosition : ISupremeRuinSystemProvider
    {
        bool TryGetPlayerPosition(string playerId, out RuinVector3 position);
    }

    public interface ISupremeRuinUi : ISupremeRuinSystemProvider
    {
        void PublishState(SupremeRuinStateSnapshot snapshot);
    }

    [Serializable]
    public sealed class ItemRequirementData { public string ItemId; public int Quantity; }

    [Serializable]
    public sealed class CombatDeathProof
    {
        public string ServerProofId;
        public string SpawnId;
        public string KillerPlayerId;
        public long ServerTimestamp;
        public bool Legitimate;
    }

    [Serializable]
    public sealed class DivineCreatureSpawnRequest
    {
        public string TrialId;
        public string RuinId;
        public string DefinitionId;
        public string GodId;
        public string Element;
        public int RequiredIndex;
        public int Level;
        public DivineCreatureStatProfile Stats;
        public string ArenaRole;
        public string SpawnPointId;
        public string InstanceId;
    }
}
