using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.AdventureWorld
{
    public class AdventureWorldGenerator : MonoBehaviour
    {
        public AdventureWorldSystem worldSystem;
        public string generationVersion = "1.0.0";
        public List<WorldRegionData> regions = new();
        public bool generatePlayerSpawnPoints = true;
        public bool generateSettlementContent = true;
        public bool generateMonsterDistribution = true;

        public void GenerateSelectedWorld()
        {
            if (worldSystem == null) worldSystem = GetComponent<AdventureWorldSystem>();
            if (worldSystem == null) throw new InvalidOperationException("AdventureWorldSystem is required.");

            var region = new WorldRegionData
            {
                id = worldSystem.worldId,
                displayName = worldSystem.displayName,
                type = worldSystem.worldType,
                placementMode = worldSystem.placementMode,
                direction = worldSystem.direction,
                targetRegionId = worldSystem.targetRegionId,
                playable = true,
                worldPosition = transform.position
            };
            UpsertRegion(region);
            InvokeProviders(region, ContentGenerationMode.FullNewWorld);
        }

        public void GenerateNewContentOnly()
        {
            if (worldSystem == null) worldSystem = GetComponent<AdventureWorldSystem>();
            foreach (var region in regions) InvokeProviders(region, ContentGenerationMode.NewContentOnly);
        }

        void UpsertRegion(WorldRegionData data)
        {
            int index = regions.FindIndex(x => x.id == data.id);
            if (index >= 0) regions[index] = data; else regions.Add(data);
        }

        void InvokeProviders(WorldRegionData region, ContentGenerationMode mode)
        {
            var skillSystem = GetComponent<AdventureWorldSkillSystem>();
            if (skillSystem != null)
            {
                skillSystem.enableSkillPavilion = worldSystem.enableSkillPavilion;
                skillSystem.SyncFromProviders();
            }

            foreach (var c in worldSystem.systemConnections)
            {
                if (c == null || c.connectedSystem == null) continue;
                if (c.connectedSystem is IAdventureWorldGenerationProvider provider)
                    provider.GenerateAdventureContent(region, mode);
                if (c.connectedSystem is IAdventureWorldTerrainProvider terrain) terrain.PrepareRegion(region);
                if (c.connectedSystem is IAdventureWorldInfrastructureProvider infrastructure) infrastructure.BuildForRegion(region);
                if (c.connectedSystem is IAdventureWorldNPCProvider npc) npc.RequestNPCs(region);
                if (c.connectedSystem is IAdventureWorldMonsterProvider monster) monster.RequestMonsterPopulation(region);
                if (c.connectedSystem is IAdventureWorldWeatherProvider weather) weather.ConfigureRegionWeather(region);
                if (c.connectedSystem is IAdventureWorldHousingProvider housing) housing.RegisterPropertySupport(region);
                if (c.connectedSystem is IAdventureWorldGuildProvider guild) guild.RegisterGuildSupport(region);
                if (c.connectedSystem is IAdventureWorldPartyProvider party) party.RegisterPartySupport(region);
            }
        }
    }
}
