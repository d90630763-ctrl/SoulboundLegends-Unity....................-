using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.AdventureWorld
{
    public class AdventureWorldArtifactSystem : MonoBehaviour
    {
        public List<ArtifactRecord> registry = new();
        public string generationVersion = "1.0.0";
        public bool GenerateNewArtifact(ArtifactRecord record)
        {
            if (record == null || string.IsNullOrEmpty(record.artifactId)) return false;
            if (registry.Exists(x => x.artifactId == record.artifactId)) return false;
            record.generationVersion = generationVersion; registry.Add(record); return true;
        }
        public bool ServerValidatedClaim(string playerId, string artifactId, IAdventureWorldServerAuthorizationProvider server)
        {
            var item = registry.Find(x => x.artifactId == artifactId);
            if (item == null || !string.IsNullOrEmpty(item.ownerId)) return false;
            if (server == null || !server.Validate("ArtifactClaim", playerId, artifactId)) return false;
            item.ownerId = playerId; item.serverVerified = true; return true;
        }
    }
}
