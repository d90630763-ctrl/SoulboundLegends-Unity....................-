using UnityEngine;

namespace SoulboundLegends.AdventureWorld
{
    public class AdventureWorldRuntime : MonoBehaviour
    {
        public AdventureWorldSystem system;
        public AdventureWorldPlayerState playerState;
        public AdventureWorldInstanceSystem instances;
        public AdventureWorldDayNightSystem dayNight;

        public void Initialize(AdventureWorldSystem source)
        {
            system = source;
            instances = GetComponent<AdventureWorldInstanceSystem>() ?? gameObject.AddComponent<AdventureWorldInstanceSystem>();
            dayNight = GetComponent<AdventureWorldDayNightSystem>() ?? gameObject.AddComponent<AdventureWorldDayNightSystem>();
            dayNight.Initialize();
        }

        public void EnterWorld(string playerId)
        {
            if (system == null) return;
            if (playerState == null) playerState = GetComponent<AdventureWorldPlayerState>() ?? gameObject.AddComponent<AdventureWorldPlayerState>();
            playerState.LoadOrCreate(playerId);
            string instanceId = instances.AssignPersistentInstance(playerId, system.worldId, system.worldType, system.maxPlayersPerVillage);
            playerState.state.currentInstanceId = instanceId;
            playerState.state.currentRegionId = system.worldId;
            playerState.Save();
            LoadSavedPlayer(playerId);
        }

        void LoadSavedPlayer(string playerId)
        {
            foreach (var slot in system.systemConnections)
            {
                if (slot?.connectedSystem is IAdventureWorldPlayerProvider provider)
                {
                    GameObject player = provider.LoadSavedPlayer(playerId);
                    if (player != null)
                    {
                        if (system.playerScript != null) system.playerScript.enabled = true;
                        return;
                    }
                }
            }
            if (system.playerCharacter != null)
            {
                var player = Instantiate(system.playerCharacter);
                player.transform.position = transform.position;
                if (system.characterController != null) system.characterController.enabled = true;
            }
        }
    }
}
