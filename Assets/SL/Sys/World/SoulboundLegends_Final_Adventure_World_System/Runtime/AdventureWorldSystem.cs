using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.AdventureWorld
{
    public class AdventureWorldSystem : MonoBehaviour
    {
        [Header("WORLD TYPE")]
        public AdventureWorldType worldType = AdventureWorldType.Village;
        public string worldId = "adventure_world_001";
        public string displayName = "New Adventure World";

        [Header("PLACEMENT")]
        public WorldPlacementMode placementMode = WorldPlacementMode.ConnectedFixed;
        public WorldDirection direction = WorldDirection.North;
        public string targetRegionId;
        public bool blendEdges = true;
        public bool matchHeights = true;
        public bool preserveWaterContinuity = true;

        [Header("VILLAGE INSTANCE")]
        public int maxPlayersPerVillage = 35;
        public bool persistVillageAssignment = true;

        [Header("PLAYER")]
        public MonoBehaviour playerScript;
        public GameObject playerCharacter;
        public MonoBehaviour characterCustomization;
        public MonoBehaviour characterController;
        public Camera playerCamera;
        public MonoBehaviour generalHUD;
        public bool loadSavedCustomizedCharacter = true;

        [Header("LEVEL 10 CITY GRADUATION")]
        public int cityUnlockLevel = 10;
        public string cityOrKingdomTokenId = "CITY_TOKEN";
        public bool permanentCityStartingPoint = true;
        public bool requirePortalSystem = true;
        public bool autoLinkVillageToCity = false;

        [Header("EXPANSION")]
        public List<AdventureWorldSystemSlot> systemConnections = new();
        public List<AdventureWorldSystemSlot> worldContentConnections = new();
        public List<AdventureWorldSystemSlot> buildingConnections = new();
        public List<AdventureWorldSystemSlot> locationConnections = new();
        public List<AdventureWorldSystemSlot> interactiveObjectConnections = new();
        public List<AdventureWorldSystemSlot> specialAreaConnections = new();
        public List<AdventureWorldSystemSlot> monsterConnections = new();
        public List<AdventureWorldSystemSlot> tamingConnections = new();
        public List<AdventureWorldSystemSlot> growthConnections = new();
        public List<AdventureWorldSystemSlot> beastFacilityConnections = new();
        public List<AdventureWorldSystemSlot> propertyConnections = new();
        public List<AdventureWorldSystemSlot> guildConnections = new();
        public List<AdventureWorldSystemSlot> weatherConnections = new();
        public List<AdventureWorldSystemSlot> skillConnections = new();

        [Header("CONTENT")]
        public bool enableArtifacts = true;
        public bool enableMaterials = true;
        public bool enableAuctionHouse = true;
        public bool enableHousing = true;
        public bool enableGuilds = true;
        public bool enableExoticBeasts = true;
        public bool enableDynamicWeather = true;
        public bool enableSkillPavilion = true;
        public bool enableWorldSkillDiscoveries = true;
        public bool enableFutureContent = true;

        [Header("RULES")]
        public bool adventureWorldIsPermanentProgressionSource = true;
        public bool preserveExistingContentOnUpdates = true;
        public bool serverAuthoritative = true;

        public AdventureWorldRuntime Runtime { get; private set; }

        public void Initialize()
        {
            Runtime = GetComponent<AdventureWorldRuntime>();
            if (Runtime == null) Runtime = gameObject.AddComponent<AdventureWorldRuntime>();
            Runtime.Initialize(this);
        }

        public void EnterWorld(string playerId)
        {
            if (Runtime == null) Initialize();
            Runtime.EnterWorld(playerId);
        }
    }
}
