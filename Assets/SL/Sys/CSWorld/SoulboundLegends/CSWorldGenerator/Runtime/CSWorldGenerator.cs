using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SoulboundLegends.CSGenerator
{
    public class CSWorldGenerator : MonoBehaviour
    {
        [Header("FINAL CS WORLD GENERATOR")]
        public CSGenerationRequest request = new CSGenerationRequest();

        [Header("Connect the existing systems here")]
        public List<CSGeneratorSystemSlot> systems = new List<CSGeneratorSystemSlot>();

        [Header("Generated World")]
        public Transform generatedWorldRoot;
        public CSGeneratedWorldData generatedData;

        public event Action<CSGenerationReport> GenerationFinished;

        private readonly string[] mapNames =
        {
            "Verdant Reach", "Fallen Dominion", "Sunscorch Expanse",
            "Frostveil Range", "Abyssal Reach"
        };

        private readonly string[] mapTypes =
        {
            "Grasslands", "Ruins", "Desert", "Snow", "Ocean"
        };

        [ContextMenu("Generate Final CS World")]
        public void GenerateFinalCSWorld()
        {
            var report = new CSGenerationReport();
            var errors = new List<string>();

            if (!ValidateBeforeGeneration(errors))
            {
                report.errors = errors.Count;
                report.summary = "Generation blocked:\n" + string.Join("\n", errors);
                Debug.LogError(report.summary, this);
                GenerationFinished?.Invoke(report);
                return;
            }

            if (generatedWorldRoot != null)
                DestroyImmediate(generatedWorldRoot.gameObject);

            var root = new GameObject(request.worldName.Replace(" ", "_"));
            root.transform.SetParent(transform);
            generatedWorldRoot = root.transform;

            generatedData = root.AddComponent<CSGeneratedWorldData>();
            generatedData.Initialize(request);

            var context = new CSGenerationContext
            {
                worldRoot = root,
                generator = this,
                request = request,
                report = report
            };

            context.maps = BuildMapDefinitions(root.transform);

            // The generator deliberately calls every connected generation-capable system.
            foreach (var slot in systems)
            {
                if (slot == null || slot.systemReference == null || !slot.useDuringGeneration)
                {
                    report.systemsSkipped++;
                    continue;
                }

                if (slot.systemReference is ICSWorldGenerationProvider provider)
                {
                    try
                    {
                        if (provider.CanGenerate(slot.type))
                        {
                            if (provider.GenerateForCS(context, request, out var providerReport))
                                report.systemsUsed++;
                            else
                                report.systemsSkipped++;
                            if (!string.IsNullOrWhiteSpace(providerReport))
                                generatedData.AddLog(slot.displayName + ": " + providerReport);
                        }
                        else
                        {
                            report.systemsSkipped++;
                            generatedData.AddLog(slot.displayName + ": provider cannot generate for this capability.");
                        }
                    }
                    catch (Exception ex)
                    {
                        report.errors++;
                        errors.Add(slot.displayName + ": " + ex.Message);
                    }
                }
                else
                {
                    // Existing systems that do not implement the interface are still retained
                    // as connected dependencies. The generated world stores their assignment.
                    generatedData.AddConnectedSystem(slot);
                }
            }

            ApplyBuiltInCSRules(context);
            report.mapsGenerated = context.maps.Length;
            int siteTotal = 0;
            foreach (var m in context.maps) siteTotal += m.sites == null ? 0 : m.sites.Length;
            report.sitesGenerated = siteTotal;
            report.completed = report.errors == 0;
            report.summary = BuildSummary(context, errors);

            if (report.errors > 0)
                Debug.LogError(report.summary, this);
            else
                Debug.Log("[Soulbound Legends CS] Final CS World generated: " + report.mapsGenerated + " maps / " + report.sitesGenerated + " sites.", this);

            GenerationFinished?.Invoke(report);
        }

        private bool ValidateBeforeGeneration(List<string> errors)
        {
            if (request == null) errors.Add("Generation Request is missing.");
            if (request != null)
            {
                if (request.mapCount != 5) errors.Add("Launch CS requires exactly 5 parent maps.");
                if (request.sitesPerMap != 7) errors.Add("Each parent map must generate exactly 7 sites.");
                if (request.playersPerTeam != 5) errors.Add("Launch CS is configured for 5 players per team.");
                if (request.roundsToWin != 5 || request.maximumRounds != 9)
                    errors.Add("Recommended launch rules are first-to-5 with a 9-round maximum.");
            }

            if (systems == null || systems.Count == 0)
                errors.Add("Connect the required shared systems before generation.");

            if (systems != null)
            {
                foreach (var slot in systems)
                {
                    if (slot != null && slot.required && slot.useDuringGeneration && slot.systemReference == null)
                        errors.Add("Missing required system: " + slot.displayName);
                }
            }
            return errors.Count == 0;
        }

        private CSMapBuildData[] BuildMapDefinitions(Transform parent)
        {
            var result = new CSMapBuildData[5];

            for (int m = 0; m < 5; m++)
            {
                var mapObject = new GameObject("MAP_" + (m + 1).ToString("00") + "_" + mapNames[m].Replace(" ", "_"));
                mapObject.transform.SetParent(parent);

                var map = new CSMapBuildData
                {
                    mapName = mapNames[m],
                    mapType = mapTypes[m],
                    parentProperties = ParentProperties(mapTypes[m]),
                    sites = new CSGeneratedSiteData[7]
                };

                for (int s = 0; s < 7; s++)
                {
                    map.sites[s] = BuildSite(map, s);
                    var siteObject = new GameObject("SITE_" + (s + 1).ToString("00") + "_" + Safe(map.sites[s].siteName));
                    siteObject.transform.SetParent(mapObject.transform);
                    siteObject.transform.localPosition = Vector3.zero;
                    var siteMarker = siteObject.AddComponent<CSGeneratedSiteMarker>();
                    siteMarker.data = map.sites[s];
                }
                result[m] = map;
            }
            return result;
        }

        private CSGeneratedSiteData BuildSite(CSMapBuildData map, int index)
        {
            string[] suffix =
            {
                "Frontier", "Citadel", "Passage", "Hollow", "Sanctum", "Crossroads", "Summit"
            };

            bool ocean = map.mapType == "Ocean";
            return new CSGeneratedSiteData
            {
                siteName = map.mapName + " — " + suffix[index],
                identity = "Combat site derived from " + map.mapName + " (" + map.mapType + ").",
                environmentInstruction = "Use the Terrain/Environment system with parent properties: " + map.parentProperties,
                infrastructureInstruction = "Use the Infrastructure system to create combat cover, routes, structures and tactical sightlines consistent with " + map.mapName,
                underwater = ocean && (index == 3 || index == 5),
                floating = ocean && (index == 1),
                combatRadius = 150f + index * 10f,
                spawnA = new Vector3(-60f, 0f, 0f),
                spawnB = new Vector3(60f, 0f, 0f)
            };
        }

        private string ParentProperties(string type)
        {
            switch (type)
            {
                case "Grasslands": return "Rolling terrain, natural cover, hills, grass, sparse woodland, streams, open lanes and varied elevation.";
                case "Ruins": return "Ancient structures, broken walls, towers, courtyards, tunnels, rubble, vertical cover and narrow routes.";
                case "Desert": return "Sand, dunes, dry valleys, cliffs, rock formations, caves, heat haze and long sightlines.";
                case "Snow": return "Snowfields, ice, frozen structures, mountains, caves, cold atmosphere and slippery terrain.";
                case "Ocean": return "Sea surface, floating structures, ships, reefs, water physics, underwater terrain, submerged structures and transition zones.";
                default: return "Fantasy combat environment.";
            }
        }

        private void ApplyBuiltInCSRules(CSGenerationContext context)
        {
            generatedData.playersPerTeam = 5;
            generatedData.roundsToWin = 5;
            generatedData.maximumRounds = 9;
            generatedData.roundDurationSeconds = 300f;
            generatedData.eliminationOnly = true;
            generatedData.captureObjectives = false;
            generatedData.siegeObjectives = false;
            generatedData.midRoundRespawn = false;
            generatedData.adventureWorldEquipment = true;
            generatedData.adventureWorldPower = true;
            generatedData.permanentEquipmentLoss = false;
            generatedData.maps = context.maps;
            generatedData.AddLog("Core CS rules applied.");
            generatedData.AddLog("General HUD is shared; CS contributes round, score, timer, alive-player and result data.");
            generatedData.AddLog("Adventure World equipment/power are used and server-validated.");
        }

        private string BuildSummary(CSGenerationContext context, List<string> errors)
        {
            var sb = new StringBuilder();
            sb.AppendLine("SOULBOUND LEGENDS — FINAL CS WORLD GENERATION");
            sb.AppendLine("5 parent maps × 7 sites = 35 combat sites.");
            sb.AppendLine("5v5 pure elimination; first to 5 rounds; maximum 9 rounds.");
            sb.AppendLine("Adventure World equipment and power are used.");
            sb.AppendLine("Terrain + Infrastructure + connected providers are invoked when they implement the generation contract.");
            sb.AppendLine("Connected non-provider systems are preserved as integration dependencies.");
            if (errors.Count > 0) sb.AppendLine("Errors: " + errors.Count);
            return sb.ToString();
        }

        private string Safe(string value)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                value = value.Replace(c, '_');
            return value.Replace(" ", "_");
        }

        public void Generate() => GenerateFinalCSWorld();
    }
}
