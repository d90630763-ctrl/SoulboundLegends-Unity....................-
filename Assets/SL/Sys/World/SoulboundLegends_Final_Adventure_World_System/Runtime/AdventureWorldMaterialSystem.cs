using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.AdventureWorld
{
    public class AdventureWorldMaterialSystem : MonoBehaviour
    {
        public List<MaterialRecord> registry = new();
        public string generationVersion = "1.0.0";
        public bool AddMaterial(MaterialRecord record)
        {
            if (record == null || string.IsNullOrEmpty(record.materialId)) return false;
            if (registry.Exists(x => x.materialId == record.materialId)) return false;
            record.generationVersion = generationVersion; registry.Add(record); return true;
        }
        public bool ValidateGather(string playerId, string materialId, int quantity, IAdventureWorldServerAuthorizationProvider server)
        {
            var material = registry.Find(x => x.materialId == materialId);
            if (material == null || quantity <= 0 || server == null) return false;
            return server.Validate("GatherMaterial", playerId, materialId + ":" + quantity);
        }
    }
}
