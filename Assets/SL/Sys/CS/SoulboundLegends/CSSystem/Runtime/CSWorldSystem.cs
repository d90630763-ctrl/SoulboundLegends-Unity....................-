using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.CS
{
    public enum CSMapType { Grasslands, Ruins, Desert, Snow, Ocean, Air }
    public enum CSMatchState { None, Loading, Preparing, RoundActive, RoundEnding, MatchEnding, Finished }
    public enum CSResult { None, TeamAWin, TeamBWin, Draw, Cancelled }

    [Serializable]
    public class CSSystemConnection
    {
        public string systemName;
        [TextArea(1,3)] public string purpose;
        public UnityEngine.Object systemReference;
        public bool required;
    }

    [Serializable]
    public class CSMapDefinition
    {
        public string mapName = "New CS Map";
        public CSMapType mapType = CSMapType.Grasslands;
        [TextArea(2,6)] public string mapProperties;
        [Range(1,7)] public int siteCount = 7;
        public List<CSCombatSiteDefinition> sites = new List<CSCombatSiteDefinition>();

        public void EnsureSites()
        {
            if (sites == null) sites = new List<CSCombatSiteDefinition>();
            while (sites.Count < siteCount)
                sites.Add(new CSCombatSiteDefinition { siteName = "Site " + (sites.Count + 1).ToString("00") });
            if (sites.Count > siteCount)
                sites.RemoveRange(siteCount, sites.Count - siteCount);
        }
    }

    [Serializable]
    public class CSCombatSiteDefinition
    {
        public string siteName = "Site 01";
        [TextArea(2,5)] public string siteDescription;
        public string generatedIdentity;
        public bool playable = true;
        public bool underwaterCapable;
        public bool floatingPlatform;
        [Range(10, 1000)] public float combatBoundaryRadius = 150f;
        public string spawnAProperties = "Team A spawn";
        public string spawnBProperties = "Team B spawn";
    }

    public class CSWorldSystem : MonoBehaviour
    {
        [Header("CS World")]
        public bool csEnabled = true;
        public int requiredUnlockLevel = 10;
        public int playersPerTeam = 5;
        public int roundsToWin = 5;
        public int maximumRounds = 9;
        public float roundDurationSeconds = 300f;
        public bool eliminationOnly = true;
        public bool noMidRoundRespawn = true;
        public bool equipmentComesFromAdventureWorld = true;
        public bool powerComesFromAdventureWorld = true;

        [Header("Five Main CS Maps")]
        public List<CSMapDefinition> maps = new List<CSMapDefinition>();

        [Header("Shared System Connections")]
        public List<CSSystemConnection> connectedSystems = new List<CSSystemConnection>();

        [Header("CS Managers")]
        public CSMatchManager matchManager;
        public CSTeamManager teamManager;
        public CSRoundManager roundManager;
        public CSScoringSystem scoringSystem;
        public CSSpawnManager spawnManager;
        public CSSpectatorManager spectatorManager;
        public CSMatchmakingManager matchmakingManager;
        public CSResultRewardManager resultRewardManager;
        public CSAFKDisconnectManager afkDisconnectManager;
        public CSMapSiteManager mapSiteManager;
        public CSBoundaryManager boundaryManager;
        public CSServerValidationBridge serverValidation;
        public CSIntegrationBridge integrationBridge;

        public CSMatchState State => matchManager != null ? matchManager.State : CSMatchState.None;

        private void Reset()
        {
            CreateDefaultMaps();
            CreateDefaultConnections();
        }

        private void Awake()
        {
            if (maps == null || maps.Count == 0) CreateDefaultMaps();
            foreach (var map in maps) map.EnsureSites();
            if (connectedSystems == null || connectedSystems.Count == 0) CreateDefaultConnections();
        }

        public void CreateDefaultMaps()
        {
            maps = new List<CSMapDefinition>();
            AddMap("Verdant Reach", CSMapType.Grasslands,
                "Grassland parent map. Rolling terrain, natural cover, hills, forests, streams and open sightlines.");
            AddMap("Fallen Dominion", CSMapType.Ruins,
                "Ruins parent map. Fortifications, broken cities, towers, courtyards, tunnels and vertical cover.");
            AddMap("Sunscorch Expanse", CSMapType.Desert,
                "Desert parent map. Sand, dunes, cliffs, dry valleys, caves and long sightlines.");
            AddMap("Frostveil Range", CSMapType.Snow,
                "Snow parent map. Ice, snowfields, frozen structures, mountains, caves and slippery terrain.");
            AddMap("Abyssal Reach", CSMapType.Ocean,
                "Ocean parent map. Above-water platforms, ships, reefs, floating structures and underwater combat sites.");
        }

        private void AddMap(string name, CSMapType type, string props)
        {
            var map = new CSMapDefinition { mapName = name, mapType = type, mapProperties = props, siteCount = 7 };
            map.EnsureSites();
            for (int i = 0; i < map.sites.Count; i++)
            {
                map.sites[i].siteName = name + " — Site " + (i + 1).ToString("00");
                map.sites[i].generatedIdentity = type + " site generated from " + name;
            }
            maps.Add(map);
        }

        public void CreateDefaultConnections()
        {
            connectedSystems = new List<CSSystemConnection>
            {
                Conn("General Lobby System", "Enters CS, presents CS access, and receives the player after the match.", true),
                Conn("General HUD System", "Shared gameplay HUD. CS supplies score, round, timer, alive-player and notifications.", true),
                Conn("Character System", "Loads the player's persistent character.", true),
                Conn("FOG Character / Controller System", "Movement and body control.", true),
                Conn("Combat System", "Attack, defense, dodge, damage, hit detection and death.", true),
                Conn("Skill System", "Uses the player's Adventure World skills.", true),
                Conn("Equipment System", "Uses equipment legitimately obtained in Adventure World.", true),
                Conn("Inventory / Item System", "Reads owned equipment and permitted items.", true),
                Conn("Stats / Battle Power System", "Provides level, stats and combat power for validation and balancing.", true),
                Conn("Animation System", "Shared character and combat animations.", false),
                Conn("Server Authority System", "Authoritative validation of player actions and match state.", true),
                Conn("Networking / Multiplayer System", "Synchronizes players, combat and match state.", true),
                Conn("Account / Player Data System", "Identifies the player and loads progression.", true),
                Conn("Scene / World Loading System", "Loads and unloads CS maps/sites.", true),
                Conn("Audio System", "Shared audio and voice infrastructure.", false),
                Conn("Camera System", "Gameplay and spectator cameras.", false),
                Conn("Party / Friends System", "Optional party queue integration.", false),
                Conn("Chat / Communication System", "Optional team communication integration.", false),
                Conn("Rank / Season System", "Optional CS rank and season progression.", false),
                Conn("Reward / Progression System", "Applies legitimate CS rewards after validation.", true),
                Conn("Game State System", "Global loading/preparing/playing/finished state.", true),
                Conn("Time Synchronization System", "Server-synchronized timers.", true),
                Conn("Physics / Collision System", "Movement, projectiles, collision and environment interaction.", true),
                Conn("Localization / Text System", "Map names, rules, notifications and results.", false),
                Conn("Analytics / Match History System", "Match history and statistics.", false),
                Conn("Reporting / Punishment System", "Reports, abandonment and moderation.", false),
                Conn("Anti-Cheat System", "Protects competitive integrity and validates client behavior.", true)
            };
        }

        private CSSystemConnection Conn(string n, string p, bool r)
        {
            return new CSSystemConnection { systemName = n, purpose = p, required = r };
        }

        public bool ValidateConfiguration(out List<string> errors)
        {
            errors = new List<string>();
            if (playersPerTeam < 1) errors.Add("Players per team must be at least 1.");
            if (roundsToWin < 1) errors.Add("Rounds to win must be at least 1.");
            if (maximumRounds < roundsToWin * 2 - 1) errors.Add("Maximum rounds must support the selected rounds-to-win value.");
            if (!eliminationOnly) errors.Add("CS must remain elimination-only.");
            if (!noMidRoundRespawn) errors.Add("CS requires no mid-round respawn.");
            if (!equipmentComesFromAdventureWorld) errors.Add("CS must use Adventure World equipment.");
            if (!powerComesFromAdventureWorld) errors.Add("CS must use Adventure World power.");
            if (maps == null || maps.Count != 5) errors.Add("CS launch configuration requires exactly 5 main maps.");
            else
            {
                foreach (var map in maps)
                {
                    if (map.sites == null || map.sites.Count != 7)
                        errors.Add(map.mapName + " must contain exactly 7 sites.");
                }
            }
            return errors.Count == 0;
        }
    }
}
