using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.AdventureWorld
{
    public class AdventureWorldInstanceSystem : MonoBehaviour
    {
        [System.Serializable] public class Instance { public string id; public string worldId; public AdventureWorldType type; public int maxPlayers; public List<string> players = new(); }
        public List<Instance> instances = new();
        public string AssignPersistentInstance(string playerId, string worldId, AdventureWorldType type, int maxPlayers)
        {
            foreach (var i in instances)
                if (i.players.Contains(playerId)) return i.id;
            foreach (var i in instances)
                if (i.worldId == worldId && i.type == type && i.players.Count < Mathf.Max(1, maxPlayers)) { i.players.Add(playerId); return i.id; }
            var created = new Instance { id = worldId + "_" + (instances.Count + 1).ToString("000"), worldId = worldId, type = type, maxPlayers = Mathf.Max(1, maxPlayers) };
            created.players.Add(playerId); instances.Add(created); return created.id;
        }
    }
}
