using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.WorldNPC
{
    public class WorldNPCAssemblyGenerator : MonoBehaviour
    {
        [Header("GENERAL")]
        public NPCSystemRegistry systemRegistry;
        public NPCOccupationDatabase occupationDatabase;
        public NPCContentDatabase contentDatabase;
        public NPCDialogueDatabase dialogueDatabase;
        public Transform generatedParent;
        public GameObject npcPrefab;
        public NPCGenerationType generationType = NPCGenerationType.Primary;

        [Header("POPULATION")]
        public List<PopulationEntry> population = new();
        [Header("PRIMARY NPCs")]
        public List<NPCBuildRequest> primaryNPCs = new();

        [System.Serializable] public class PopulationEntry { public string occupationId; [Min(0)] public int count = 1; }
        [System.Serializable] public class NPCBuildRequest { public string npcId; public string displayName; public string occupationId; public NPCImportance importance = NPCImportance.Important; }

        public void GeneratePrimaryNPCs() { foreach (var r in primaryNPCs) CreateNPC(r.occupationId, r.displayName, r.npcId, r.importance); }
        public void GenerateSecondaryNPCs() { foreach (var p in population) for (int i=0;i<p.count;i++) CreateNPC(p.occupationId, null, null, NPCImportance.Normal); }

        private NPCController CreateNPC(string occupationId, string name, string id, NPCImportance importance)
        {
            if (!npcPrefab) { Debug.LogError("World NPC Generator: NPC Prefab is not assigned."); return null; }
            var go = Instantiate(npcPrefab, generatedParent ? generatedParent : transform);
            var npc = go.GetComponent<NPCController>(); if (!npc) npc = go.AddComponent<NPCController>();
            npc.registry = systemRegistry ? systemRegistry : GetComponentInParent<NPCSystemRegistry>();
            int seed = System.Guid.NewGuid().GetHashCode(); Random.InitState(seed);
            npc.identity.npcId = string.IsNullOrEmpty(id) ? $"NPC_{System.Guid.NewGuid():N}" : id;
            npc.identity.displayName = string.IsNullOrEmpty(name) ? GenerateName() : name;
            npc.identity.occupationId = occupationId;
            npc.identity.importance = importance;
            npc.identity.gender = Random.value < .5f ? NPCGender.Male : NPCGender.Female;
            npc.identity.age = Random.Range(18, 75);
            npc.identity.uniqueSeed = seed;
            npc.identity.personality = RandomPersonality();
            ApplyOccupationRules(npc);
            AssignVoice(npc);
            return npc;
        }

        private void ApplyOccupationRules(NPCController npc)
        {
            npc.services.Clear();
            var def = occupationDatabase ? occupationDatabase.Find(npc.identity.occupationId) : null;
            if (def != null)
            {
                foreach (var service in def.allowedServices) npc.services.Add(new NPCServiceBinding { service = service, enabled = true });
                foreach (var forbidden in def.forbiddenCapabilities) { /* capability restrictions are enforced at use time */ }
            }
            else
            {
                npc.services.Add(new NPCServiceBinding { service = NPCServiceType.Talk, enabled = true });
            }
        }

        private void AssignVoice(NPCController npc)
        {
            if (!npc.registry) return;
            var voice = npc.registry.Find<INPCVoiceProvider>();
            if (voice != null) voice.TryAssignVoice(npc, npc.identity.gender, npc.identity.uniqueSeed);
        }

        private static string GenerateName() => $"Villager {Random.Range(1000,9999)}";
        private static string RandomPersonality() => new[] { "Friendly", "Serious", "Quiet", "Energetic", "Cautious", "Cheerful", "Reserved", "Curious" }[Random.Range(0,8)];
    }
}
