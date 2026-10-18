using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.WorldNPC
{
    public static class NPCDialogueRuntime
    {
        public static void SpeakApproach(NPCController npc, GameObject player)
        {
            if (!npc) return;
            var provider = npc.registry ? npc.registry.Find<INPCDialogueProvider>() : null;
            var ctx = new NPCDialogueContext { firstMeeting = true, hasActiveQuest = npc.HasService(NPCServiceType.Quest) };
            string line = provider != null ? provider.GetApproachLine(npc, ctx) : DefaultGreeting(npc);
            if (string.IsNullOrWhiteSpace(line)) line = DefaultGreeting(npc);
            var voice = npc.registry ? npc.registry.Find<INPCVoiceProvider>() : null;
            if (voice != null) voice.Speak(npc, line);
            else Debug.Log($"NPC [{npc.identity.displayName}] says: {line}");
        }

        private static string DefaultGreeting(NPCController npc)
        {
            string o = npc.identity.occupationId?.ToLowerInvariant() ?? "villager";
            if (o.Contains("blacksmith")) return "Need something repaired?";
            if (o.Contains("alchemist")) return "Looking for a potion?";
            if (o.Contains("guard")) return "State your business.";
            if (o.Contains("guild")) return "Welcome to the Adventure Guild.";
            if (o.Contains("farmer")) return "Good day, traveler.";
            if (o.Contains("merchant")) return "Take a look at my wares.";
            if (o.Contains("noble")) return "Do you have business with me?";
            if (o.Contains("child")) return "Are you an adventurer?";
            return "Hello, traveler.";
        }
    }

    public class DefaultDialogueProvider : MonoBehaviour, INPCSystemCapabilityProvider, INPCDialogueProvider
    {
        public NPCDialogueDatabase database;
        public void GetCapabilities(List<NPCSystemCapability> capabilities) => capabilities.Add(NPCSystemCapability.Dialogue);
        public bool CanHandle(NPCSystemCapability capability) => capability == NPCSystemCapability.Dialogue;
        public string GetApproachLine(NPCController npc, NPCDialogueContext context)
        {
            if (!database) return string.Empty;
            foreach (var tag in npc.approachDialogueTags) { var line = database.GetRandom(tag); if (!string.IsNullOrEmpty(line)) return line; }
            return string.Empty;
        }
    }
}
