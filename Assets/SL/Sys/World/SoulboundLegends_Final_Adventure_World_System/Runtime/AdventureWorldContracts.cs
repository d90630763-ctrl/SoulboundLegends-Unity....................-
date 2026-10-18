using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.AdventureWorld
{
    public enum AdventureWorldType { Village, Town, City, Kingdom }
    public enum WorldPlacementMode { ConnectedFixed, SeparateFloating }
    public enum WorldDirection { North, South, East, West }
    public enum ContentGenerationMode { FullNewWorld, NewContentOnly }
    public enum CreatureKind { Monster, ExoticBeast }
    public enum MaterialType { Ore, Plant, Crystal, Water, Wood, Beast, Monster, Magic, Other }
    public enum ArtifactGrade { Common, Uncommon, Rare, Epic, Legendary, Mythic, Divine, GodTier }

    [Serializable] public class AdventureWorldSystemSlot
    {
        public string systemName;
        public string capability;
        public UnityEngine.Object connectedSystem;
    }

    [Serializable] public class WorldRegionData
    {
        public string id;
        public string displayName;
        public AdventureWorldType type;
        public WorldPlacementMode placementMode;
        public WorldDirection direction;
        public string targetRegionId;
        public bool playable = true;
        public Vector3 worldPosition;
    }

    [Serializable] public class PlayerWorldState
    {
        public string playerId;
        public string currentRegionId;
        public string currentInstanceId;
        public string permanentCityStartRegionId;
        public Vector3 savedPosition;
        public int level = 1;
        public long xp;
        public int skillPoints;
        public string customizationId;
    }

    [Serializable] public class ArtifactRecord
    {
        public string artifactId;
        public string name;
        public ArtifactGrade grade;
        public string source;
        public string regionId;
        public Vector3 position;
        public bool hidden = true;
        public string requiredMonsterId;
        public int requiredLevel;
        public float dropChance = 0.01f;
        public string ownerId;
        public string generationVersion;
        public bool serverVerified;
    }

    [Serializable] public class MaterialRecord
    {
        public string materialId;
        public string name;
        public ArtifactGrade grade;
        public MaterialType type;
        public string regionId;
        public string biome;
        public int spawnQuantity;
        public float respawnSeconds;
        public int requiredLevel;
        public string generationVersion;
    }

    [Serializable] public class AuctionRecord
    {
        public string auctionId;
        public string materialId;
        public int quantity;
        public string sellerId;
        public long priceAether;
        public string buyerId;
        public string status;
        public bool serverValidated;
    }

    public interface IAdventureWorldTerrainProvider
    { bool CanProvideTerrain { get; } void PrepareRegion(WorldRegionData region); }
    public interface IAdventureWorldInfrastructureProvider
    { bool CanProvideInfrastructure { get; } void BuildForRegion(WorldRegionData region); }
    public interface IAdventureWorldNPCProvider
    { void RequestNPCs(WorldRegionData region); }
    public interface IAdventureWorldMonsterProvider
    { void RequestMonsterPopulation(WorldRegionData region); }
    public interface IAdventureWorldPortalProvider
    { bool CanConnectPortals { get; } bool RequestPortal(string fromRegionId, string toRegionId); }
    public interface IAdventureWorldPlayerProvider
    { GameObject LoadSavedPlayer(string playerId); }
    public interface IAdventureWorldCapabilityProvider { IEnumerable<string> GetCapabilities(); }
    public interface IAdventureWorldCraftingProvider { IEnumerable<string> RequestArtifactDefinitions(int count); }
    public interface IAdventureWorldMaterialProvider { IEnumerable<string> RequestMaterialDefinitions(int count); }
    public interface IAdventureWorldServerAuthorizationProvider
    {
        bool Validate(string action, string playerId, string payload);
        bool ValidateOwnership(string playerId, string itemId);
    }
    public interface IAdventureWorldWeatherProvider { void ConfigureRegionWeather(WorldRegionData region); }
    public interface IAdventureWorldHousingProvider { void RegisterPropertySupport(WorldRegionData region); }
    public interface IAdventureWorldGuildProvider { void RegisterGuildSupport(WorldRegionData region); }
    public interface IAdventureWorldPartyProvider { void RegisterPartySupport(WorldRegionData region); }
    public interface IAdventureWorldGenerationProvider { void GenerateAdventureContent(WorldRegionData region, ContentGenerationMode mode); }
}
