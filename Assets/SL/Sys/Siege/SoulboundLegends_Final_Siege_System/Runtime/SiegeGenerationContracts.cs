using System;
using UnityEngine;
namespace SoulboundLegends.Siege {
public enum SiegeGeneratorSystemType { Terrain,Infrastructure,Water,Weather,Lighting,Destruction,NPC,Monster,WorldNPCAssembly,Character,Combat,Skills,Equipment,Inventory,StatsBattlePower,Animation,ServerAuthorization,Networking,AntiCheat,Matchmaking,TeamBalancing,GeneralHUD,Camera,Audio,PartyFriends,Communication,Rewards,RankAdvancement,RankSeason,GameState,TimeSync,Physics,Localization,Analytics,Reporting,Lobby,SceneLoading }
[Serializable] public class SiegeGeneratorSystemSlot { public string displayName; public SiegeGeneratorSystemType type; public string capability; public UnityEngine.Object reference; public bool required=true; public bool useDuringGeneration=true; public bool connected; [TextArea(1,3)] public string role; }
[Serializable] public class SiegeGenerationRequest {
public string worldName="Soulbound Legends Siege"; public int mainMapCount=5,subMapsPerMainMap=7,totalSubMaps=35;
public int minPlayers=2,maxPlayers=10,normalRounds=2; public bool rotateRoles=true,finalRoundAtOneOne=true,finalRoundSimultaneousAttackDefense=true;
public bool requireControlCore=true,destroyCoreEndsRound=true,defenderWinsOnTimer=true,defenderWinsOnCompleteAttackerWipe=true,finalFirstCoreDestroyedWins=true,capturePoints=false,csRoundRules=false;
public bool fortifiedDefenderTerritory=true,defensiveWalls=true,reinforcedGates=true,defensiveTowers=true,barricades=true,defensiveBuildings=true,chokePoints=true,alternativeAttackRoutes=true,destructibleWalls=true,destructibleBuildings=true,destructibleGates=true,protectedCoreChamber=true;
public bool generateTerrain=true,generateInfrastructure=true,generateWater=true,generateWeather=true,generateLighting=true,useDestruction=true,useNPC=true,useMonsters=true,useWorldNPCAssembly=true;
public bool useAdventureWorldEquipment=true,useAdventureWorldSkills=true,useAdventureWorldPowers=true,useAdventureWorldStats=true,permanentAdventureWorldLossOnDeath=false;
public bool useMatchmaking=true,useTeamBalancing=true,useServerAuthorization=true,useNetworking=true,useAntiCheat=true,useRankAdvancement=true,useSeason=true,useAnalytics=true,useReporting=true,useRewards=true;
public bool useGeneralHUD=true,returnToLobbyAfterMatch=true;
}
public sealed class SiegeGenerationContext { public SiegeWorldGenerator generator; public SiegeGenerationRequest request; public SiegeMapBuildData map; public SiegeGeneratedWorldData world; }
[Serializable] public class SiegeMapBuildData { public int mainMapIndex,subMapIndex; public string mainMapName,subMapName,environment,defensiveTheme,terrainBrief,infrastructureBrief,coreBrief,destructionBrief,attackRouteBrief,defenseBrief; }
public interface ISiegeWorldGenerationProvider { bool CanGenerate(SiegeGeneratorSystemType type); bool GenerateForSiege(SiegeGenerationContext context,SiegeGenerationRequest request,out string report); }
}