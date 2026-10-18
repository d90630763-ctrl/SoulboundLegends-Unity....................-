using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.AdventureWorld
{
    public class AdventureWorldContentRegistry : MonoBehaviour
    {
        public string contentVersion = "1.0.0";
        public List<string> persistentWorldIds = new();
        public List<string> persistentArtifactIds = new();
        public List<string> persistentMaterialIds = new();
        public List<string> persistentAuctionIds = new();
        public bool PreserveExisting(string id) => !string.IsNullOrEmpty(id) && persistentWorldIds.Contains(id);
        public void RegisterWorld(string id) { if (!string.IsNullOrEmpty(id) && !persistentWorldIds.Contains(id)) persistentWorldIds.Add(id); }
        public void RegisterArtifact(string id) { if (!string.IsNullOrEmpty(id) && !persistentArtifactIds.Contains(id)) persistentArtifactIds.Add(id); }
        public void RegisterMaterial(string id) { if (!string.IsNullOrEmpty(id) && !persistentMaterialIds.Contains(id)) persistentMaterialIds.Add(id); }
        public void RegisterAuction(string id) { if (!string.IsNullOrEmpty(id) && !persistentAuctionIds.Contains(id)) persistentAuctionIds.Add(id); }
    }
}
