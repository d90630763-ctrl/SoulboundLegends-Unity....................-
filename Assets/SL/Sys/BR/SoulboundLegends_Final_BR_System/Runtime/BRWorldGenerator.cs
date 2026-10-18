using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.BR
{
    public sealed class BRWorldGenerator : MonoBehaviour
    {
        [Header("FINAL BR WORLD GENERATOR")]
        public BRGenerationRequest request = new BRGenerationRequest();

        [Header("CONNECTED SYSTEMS")]
        public List<BRGeneratorSystemSlot> systems = new List<BRGeneratorSystemSlot>();

        [Header("OUTPUT")]
        public BRGeneratedWorldData generatedWorld;

        public void AutoCreateConnectionSlots()
        {
            systems.Clear();
            Add("General Lobby System", BRGeneratorSystemType.Lobby, true, "Lobby entry/exit and mode selection.");
            Add("General HUD System", BRGeneratorSystemType.HUD, true, "Shared HUD; BR feeds BR information into it.");
            Add("Character System", BRGeneratorSystemType.Character, true, "Uses the player's existing character.");
            Add("Character Controller System", BRGeneratorSystemType.CharacterController, true, "Movement and BR traversal.");
            Add("Combat System", BRGeneratorSystemType.Combat, true, "BR combat.");
            Add("Skill System", BRGeneratorSystemType.Skills, true, "Adventure World skills only.");
            Add("Equipment System", BRGeneratorSystemType.Equipment, true, "Adventure World equipment only.");
            Add("Inventory / Item System", BRGeneratorSystemType.Inventory, true, "Persistent and match item handling.");
            Add("Stats / Battle Power System", BRGeneratorSystemType.StatsBattlePower, true, "Adventure World stats/battle power.");
            Add("Animation System", BRGeneratorSystemType.Animation, true, "Character and effects.");
            Add("Server Authority System", BRGeneratorSystemType.ServerAuthority, true, "Server validation.");
            Add("Networking / Multiplayer System", BRGeneratorSystemType.Networking, true, "Multiplayer synchronization.");
            Add("Account / Player Data System", BRGeneratorSystemType.AccountData, true, "Persistent player data.");
            Add("Scene / World Loading System", BRGeneratorSystemType.SceneLoader, true, "Loads generated BR worlds.");
            Add("Audio System", BRGeneratorSystemType.Audio, true, "BR audio.");
            Add("Camera System", BRGeneratorSystemType.Camera, true, "BR camera.");
            Add("Party / Friends System", BRGeneratorSystemType.PartyFriends, false, "Duo/squad parties.");
            Add("Chat / Communication System", BRGeneratorSystemType.Chat, false, "Team communication.");
            Add("Rank / Season System", BRGeneratorSystemType.RankSeason, false, "BR rank/season integration.");
            Add("Reward / Progression System", BRGeneratorSystemType.RewardsProgression, true, "Post-match rewards.");
            Add("Game State System", BRGeneratorSystemType.GameState, true, "Match state.");
            Add("Time Synchronization System", BRGeneratorSystemType.TimeSync, true, "Server timing.");
            Add("Physics / Collision System", BRGeneratorSystemType.PhysicsCollision, true, "World and combat physics.");
            Add("Localization / Text System", BRGeneratorSystemType.Localization, false, "UI text.");
            Add("Analytics / Match History System", BRGeneratorSystemType.AnalyticsHistory, false, "Match statistics/history.");
            Add("Reporting / Punishment System", BRGeneratorSystemType.ReportingModeration, false, "Reports and abandonment.");
            Add("Anti-Cheat System", BRGeneratorSystemType.AntiCheat, true, "Anti-cheat validation.");

            Add("Terrain Generator", BRGeneratorSystemType.TerrainGenerator, true, "Creates the five massive BR terrain worlds.");
            Add("Infrastructure Generator", BRGeneratorSystemType.InfrastructureGenerator, true, "Creates buildings, settlements and structures.");
            Add("Water / Environment System", BRGeneratorSystemType.WaterEnvironment, true, "Creates lakes, rivers, oceans and related environments.");
            Add("Weather / Lighting System", BRGeneratorSystemType.WeatherLighting, false, "Map atmosphere and weather.");
            Add("Terrain Destruction System", BRGeneratorSystemType.TerrainDestruction, false, "Destructible BR environment.");
            Add("NPC Generator", BRGeneratorSystemType.NPCGenerator, false, "World NPC generation.");
            Add("Monster Generator", BRGeneratorSystemType.MonsterGenerator, false, "World monster generation.");
            Add("World NPC Assembly System", BRGeneratorSystemType.WorldNPCAssembly, false, "Assembles generated NPCs.");
            Add("Character Model Generator", BRGeneratorSystemType.CharacterModelGenerator, false, "Generated character assets where used.");
            Add("Material / Texture Generator", BRGeneratorSystemType.MaterialTextureGenerator, true, "Generated world materials and textures.");
        }

        private void Add(string name, BRGeneratorSystemType type, bool required, string role)
        {
            systems.Add(new BRGeneratorSystemSlot {
                displayName = name, type = type, required = required,
                useDuringGeneration = true, generationRole = role,
                requiredCapability = type.ToString()
            });
        }

        public bool Validate(out string report)
        {
            var errors = new List<string>();
            if (request == null) errors.Add("Generation request is missing.");
            else
            {
                if (request.mapCount != 5) errors.Add("BR must contain exactly 5 massive maps.");
                if (request.playersPerMatch < 2) errors.Add("Players per match must be at least 2.");
                if (!request.solo && !request.duo && !request.squad) errors.Add("Enable at least one BR team format.");
                if (!request.useAdventureWorldEquipment) errors.Add("BR equipment must come from Adventure World.");
                if (!request.useAdventureWorldSkills) errors.Add("BR skills must come from Adventure World.");
                if (!request.useAdventureWorldPowers) errors.Add("BR powers must come from Adventure World.");
                if (request.permanentAdventureWorldLossOnDeath) errors.Add("Permanent Adventure World equipment loss on BR death is forbidden by the BR rules.");
            }

            foreach (var s in systems)
            {
                s.connected = s.systemReference != null;
                if (s.required && !s.connected) errors.Add("Missing required system: " + s.displayName);
            }

            report = errors.Count == 0
                ? "BR validation passed."
                : "BR validation failed:\n- " + string.Join("\n- ", errors);
            return errors.Count == 0;
        }

        public bool GenerateFinalBRWorld()
        {
            if (!Validate(out var validationReport))
            {
                Debug.LogError(validationReport, this);
                return false;
            }

            if (generatedWorld != null) DestroyImmediate(generatedWorld.gameObject);

            var root = new GameObject(request.worldName + " - GENERATED");
            root.transform.SetParent(transform, false);

            generatedWorld = root.AddComponent<BRGeneratedWorldData>();
            generatedWorld.worldName = request.worldName;
            generatedWorld.rules = request;
            generatedWorld.readyForLobby = false;
            generatedWorld.readyForServerInstance = false;

            var context = new BRGenerationContext { worldRoot = root };
            context.logs.Add(validationReport);

            CreateMaps(context);

            foreach (var slot in systems)
            {
                if (!slot.connected || !slot.useDuringGeneration) continue;
                generatedWorld.connectedSystems.Add(slot.displayName);

                var provider = slot.systemReference as IBRWorldGenerationProvider;
                if (provider != null && provider.CanGenerate(slot.type))
                {
                    if (provider.GenerateForBR(context, request, out var providerReport))
                    {
                        generatedWorld.generationReports.Add(slot.displayName + ": " + providerReport);
                    }
                    else
                    {
                        generatedWorld.generationReports.Add(slot.displayName + ": provider reported failure - " + providerReport);
                        Debug.LogWarning(generatedWorld.generationReports[generatedWorld.generationReports.Count - 1], this);
                    }
                }
                else
                {
                    generatedWorld.generationReports.Add(slot.displayName + ": connected as runtime dependency; no BR generation provider interface detected.");
                }
            }

            generatedWorld.maps.AddRange(context.maps);
            CreateMarkers(root, context);
            generatedWorld.readyForServerInstance = true;
            generatedWorld.readyForLobby = true;

            Debug.Log("FINAL BR WORLD GENERATED. The generated root is ready to be connected to the Lobby.", root);
            return true;
        }

        public void Generate() => GenerateFinalBRWorld();

        private void CreateMaps(BRGenerationContext context)
        {
            var maps = new (string name, string environment, string instruction)[]
            {
                ("Aetherwild Expanse", "Grasslands / Forest / Highlands", "Massive fantasy wilderness with grasslands, forests, cliffs, rivers, settlements and exploration routes."),
                ("Kingdoms of Ruin", "Ruins / Kingdoms", "Massive ruined kingdom landscape with fortresses, ancient roads, broken cities, caves and dangerous ruins."),
                ("Sunfire Frontier", "Desert / Canyons", "Massive desert world with dunes, canyons, oases, ruins, cliffs and buried structures."),
                ("Frostbound Dominion", "Snow / Mountains", "Massive frozen world with mountains, snowfields, frozen lakes, caves, settlements and exposed high ground."),
                ("Abyssal Realm", "Ocean / Islands / Underwater", "Massive oceanic fantasy world with islands, reefs, ships, floating structures, coastlines and optional underwater regions.")
            };

            for (int i = 0; i < maps.Length; i++)
            {
                var m = maps[i];
                var data = new BRMapBuildData {
                    mapId = "BR_MAP_" + (i + 1).ToString("00"),
                    mapName = m.name,
                    environmentType = m.environment,
                    worldInstruction = m.instruction
                };

                AddPOI(data, "Settlement", "Settlement", "Exploration, equipment/resource discovery and combat routes.", true, false);
                AddPOI(data, "Ancient Ruins", "Ruins", "High-risk exploration area with valuable match resources.", true, false);
                AddPOI(data, "Wildlands", "Wilderness", "Open traversal and ambush space.", false, false);
                AddPOI(data, "Elite Territory", "Monster Territory", "Optional PvE challenge and temporary match rewards.", true, true);
                AddPOI(data, "Fortified Landmark", "Landmark", "Strong defensive terrain and high-value combat area.", true, false);
                AddPOI(data, "Hidden Region", "Secret Area", "Exploration-focused location with configurable special content.", true, false);

                for (int s = 0; s < 12; s++)
                {
                    data.teleportSpawns.Add(new BRTeleportSpawnData {
                        id = data.mapId + "_TP_" + (s + 1).ToString("00"),
                        locationPurpose = "Valid randomized BR starting location",
                        position = new Vector3((s % 4) * 40f, 0f, (s / 4) * 40f),
                        safeRadius = 20f,
                        visibleArrivalBeam = request.generateTeleportLightBeams
                    });
                }

                context.maps.Add(data);
            }
        }

        private void AddPOI(BRMapBuildData map, string name, string type, string purpose, bool highValue, bool boss)
        {
            map.pointsOfInterest.Add(new BRPointOfInterestData {
                id = map.mapId + "_" + type.Replace(" ", "_").ToUpperInvariant(),
                name = name,
                type = type,
                gameplayPurpose = purpose,
                highValue = highValue,
                bossArea = boss
            });
        }

        private void CreateMarkers(GameObject root, BRGenerationContext context)
        {
            foreach (var map in context.maps)
            {
                var mapObject = new GameObject(map.mapName);
                mapObject.transform.SetParent(root.transform, false);
                var marker = mapObject.AddComponent<BRGeneratedMapMarker>();
                marker.mapId = map.mapId;
                marker.mapName = map.mapName;
                marker.environmentType = map.environmentType;

                foreach (var poi in map.pointsOfInterest)
                {
                    var p = new GameObject(poi.name);
                    p.transform.SetParent(mapObject.transform, false);
                    var pm = p.AddComponent<BRGeneratedPOIMarker>();
                    pm.poiId = poi.id;
                    pm.poiName = poi.name;
                    pm.poiType = poi.type;
                    pm.highValue = poi.highValue;
                }
            }
        }
    }
}
