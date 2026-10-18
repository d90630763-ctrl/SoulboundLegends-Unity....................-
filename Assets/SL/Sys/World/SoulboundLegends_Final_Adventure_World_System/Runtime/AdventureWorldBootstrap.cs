using UnityEngine;

namespace SoulboundLegends.AdventureWorld
{
    public class AdventureWorldBootstrap : MonoBehaviour
    {
        public AdventureWorldSystem adventureWorld;
        public AdventureWorldGenerator generator;
        public string playerId = "LOCAL_PLAYER";
        public bool enterOnStart;
        void Start()
        {
            if (adventureWorld == null) adventureWorld = GetComponent<AdventureWorldSystem>();
            if (generator == null) generator = GetComponent<AdventureWorldGenerator>();
            if (adventureWorld != null) adventureWorld.Initialize();
            if (enterOnStart && adventureWorld != null) adventureWorld.EnterWorld(playerId);
        }
    }
}
