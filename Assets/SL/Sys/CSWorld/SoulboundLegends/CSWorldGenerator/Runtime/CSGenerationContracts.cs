using System;
using UnityEngine;

namespace SoulboundLegends.CSGenerator
{
    public enum CSGeneratorSystemType
    {
        Lobby, HUD, Character, CharacterController, Combat, Skills, Equipment, Inventory,
        StatsBattlePower, Animation, ServerAuthority, Networking, AccountData, SceneLoader,
        Audio, Camera, PartyFriends, Chat, RankSeason, RewardsProgression, GameState,
        TimeSync, PhysicsCollision, Localization, AnalyticsHistory, ReportingModeration,
        AntiCheat, TerrainGenerator, InfrastructureGenerator, WaterEnvironment,
        WeatherLighting, TerrainDestruction, NPCGenerator, MonsterGenerator, WorldNPCAssembly,
        CharacterModelGenerator, MaterialTextureGenerator
    }

    [Serializable]
    public class CSGeneratorSystemSlot
    {
        public string displayName;
        public CSGeneratorSystemType type;
        [TextArea(1,4)] public string requiredCapability;
        public UnityEngine.Object systemReference;
        public bool required = true;
        public bool useDuringGeneration = true;
        public bool connected;
        [TextArea(1,3)] public string generationRole;
    }

    public interface ICSWorldGenerationProvider
    {
        bool CanGenerate(CSGeneratorSystemType type);
        bool GenerateForCS(CSGenerationContext context, CSGenerationRequest request, out string report);
    }

    [Serializable]
    public class CSGenerationRequest
    {
        public string worldName = "Soulbound Legends CS World";
        public int mapCount = 5;
        public int sitesPerMap = 7;
        public int playersPerTeam = 5;
        public int roundsToWin = 5;
        public int maximumRounds = 9;
        public float roundDurationSeconds = 300f;
        public bool generateTerrain = true;
        public bool generateInfrastructure = true;
        public bool generateWater = true;
        public bool generateWeatherAndLighting = true;
        public bool generateDestruction = true;
        public bool generateCombatBoundaries = true;
        public bool generateSpawnLocations = true;
        public bool generateSiteIdentity = true;
        public bool generateRuleData = true;
        public bool generateHUDBindings = true;
        public bool generateServerValidationBindings = true;
        public bool generateMatchmakingBindings = true;
        public bool generateDocumentation = true;
    }

    public class CSGenerationContext
    {
        public GameObject worldRoot;
        public CSWorldGenerator generator;
        public CSGenerationRequest request;
        public CSGenerationReport report;
        public CSMapBuildData[] maps;
    }

    [Serializable]
    public class CSMapBuildData
    {
        public string mapName;
        public string mapType;
        [TextArea(2,6)] public string parentProperties;
        public CSGeneratedSiteData[] sites;
    }

    [Serializable]
    public class CSGeneratedSiteData
    {
        public string siteName;
        [TextArea(2,5)] public string identity;
        public string environmentInstruction;
        public string infrastructureInstruction;
        public bool underwater;
        public bool floating;
        public float combatRadius = 150f;
        public Vector3 spawnA;
        public Vector3 spawnB;
    }

    [Serializable]
    public class CSGenerationReport
    {
        public bool completed;
        public int mapsGenerated;
        public int sitesGenerated;
        public int systemsUsed;
        public int systemsSkipped;
        public int errors;
        [TextArea(5,30)] public string summary;
    }
}
