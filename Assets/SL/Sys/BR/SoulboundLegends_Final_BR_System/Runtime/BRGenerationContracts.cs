using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.BR
{
    public enum BRGeneratorSystemType
    {
        Lobby, HUD, Character, CharacterController, Combat, Skills, Equipment,
        Inventory, StatsBattlePower, Animation, ServerAuthority, Networking,
        AccountData, SceneLoader, Audio, Camera, PartyFriends, Chat, RankSeason,
        RewardsProgression, GameState, TimeSync, PhysicsCollision, Localization,
        AnalyticsHistory, ReportingModeration, AntiCheat, TerrainGenerator,
        InfrastructureGenerator, WaterEnvironment, WeatherLighting,
        TerrainDestruction, NPCGenerator, MonsterGenerator, WorldNPCAssembly,
        CharacterModelGenerator, MaterialTextureGenerator
    }

    [Serializable]
    public class BRGeneratorSystemSlot
    {
        public string displayName;
        public BRGeneratorSystemType type;
        public string requiredCapability;
        public MonoBehaviour systemReference;
        public bool required = true;
        public bool useDuringGeneration = true;
        public bool connected;
        [TextArea(1,3)] public string generationRole;
    }

    public interface IBRWorldGenerationProvider
    {
        bool CanGenerate(BRGeneratorSystemType type);
        bool GenerateForBR(BRGenerationContext context, BRGenerationRequest request, out string report);
    }

    [Serializable]
    public class BRGenerationRequest
    {
        public string worldName = "Soulbound Legends Battle Royale";
        public int mapCount = 5;
        public int playersPerMatch = 100;
        public int minimumPlayers = 2;

        public bool solo = true;
        public bool duo = true;
        public bool squad = true;

        public bool generateTerrain = true;
        public bool generateInfrastructure = true;
        public bool generateWater = true;
        public bool generateWeatherLighting = true;
        public bool generateDestruction = true;
        public bool generateCombatBoundaries = true;
        public bool generateTeleportSpawns = true;
        public bool generateTeleportLightBeams = true;
        public bool generateLootData = true;
        public bool generateSafeZoneData = true;
        public bool generateWorldEvents = true;
        public bool generateNPCData = true;
        public bool generateMonsterData = true;
        public bool generateHUDBindings = true;
        public bool generateServerValidationBindings = true;
        public bool generateMatchmakingBindings = true;
        public bool generateRewardsBindings = true;
        public bool generateDocumentation = true;

        public bool useAdventureWorldEquipment = true;
        public bool useAdventureWorldSkills = true;
        public bool useAdventureWorldPowers = true;
        public bool useAdventureWorldStats = true;
        public bool permanentAdventureWorldLossOnDeath = false;

        public bool enableDownedState = true;
        public bool enableRevive = true;
        public bool enableSpectating = true;

        public float initialSafeZoneRadius = 1f;
        public int safeZonePhases = 6;
        public float finalSafeZoneRadius = 0.08f;
        public float outOfZoneDamagePerSecond = 10f;

        [TextArea(2,5)] public string fantasyEntryRule =
            "Server randomly assigns a valid starting location. Player arrives through anime-style magical teleportation. A visible beam of light marks arrival.";

        [TextArea(2,5)] public string permanentProgressionRule =
            "All permanent weapons, skills, powers, equipment and character progression originate in Adventure World. BR does not create a separate permanent power progression.";
    }

    public sealed class BRGenerationContext
    {
        public GameObject worldRoot;
        public List<BRMapBuildData> maps = new List<BRMapBuildData>();
        public readonly List<string> logs = new List<string>();
    }

    [Serializable]
    public class BRMapBuildData
    {
        public string mapId;
        public string mapName;
        public string environmentType;
        [TextArea(2,6)] public string worldInstruction;
        public List<BRPointOfInterestData> pointsOfInterest = new List<BRPointOfInterestData>();
        public List<BRTeleportSpawnData> teleportSpawns = new List<BRTeleportSpawnData>();
    }

    [Serializable]
    public class BRPointOfInterestData
    {
        public string id;
        public string name;
        public string type;
        [TextArea(1,4)] public string gameplayPurpose;
        public bool highValue;
        public bool bossArea;
    }

    [Serializable]
    public class BRTeleportSpawnData
    {
        public string id;
        public string locationPurpose;
        public Vector3 position;
        public float safeRadius = 20f;
        public bool visibleArrivalBeam = true;
    }
}
