using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.WorldNPC
{
    /// <summary>Adapter example for the user's existing Voice System. Replace the internals with that system's API.</summary>
    public class ExampleVoiceSystemAdapter : MonoBehaviour, INPCSystemCapabilityProvider, INPCVoiceProvider
    {
        public List<NPCVoiceProfile> maleProfiles = new();
        public List<NPCVoiceProfile> femaleProfiles = new();
        public void GetCapabilities(List<NPCSystemCapability> c) => c.Add(NPCSystemCapability.Voice);
        public bool CanHandle(NPCSystemCapability c) => c == NPCSystemCapability.Voice;
        public bool TryAssignVoice(NPCController npc, NPCGender gender, int uniqueSeed)
        {
            var list = gender == NPCGender.Male ? maleProfiles : femaleProfiles;
            if (list != null && list.Count > 0) npc.voiceProfile = list[Mathf.Abs(uniqueSeed) % list.Count];
            return npc.voiceProfile != null;
        }
        public void Speak(NPCController npc, string text) => Debug.Log($"VOICE [{npc.voiceProfile?.name ?? "Default"}] {npc.identity.displayName}: {text}");
    }

    /// <summary>Development adapter showing occupation-gated crafting.</summary>
    public class ExampleOccupationCraftingAdapter : MonoBehaviour, INPCSystemCapabilityProvider, INPCCraftingProvider
    {
        public List<string> allowedOccupationIds = new() { "blacksmith", "guild_blacksmith", "carpenter", "artisan" };
        public void GetCapabilities(List<NPCSystemCapability> c) => c.Add(NPCSystemCapability.Crafting);
        public bool CanHandle(NPCSystemCapability c) => c == NPCSystemCapability.Crafting;
        public bool CanOccupationCraft(string id) => allowedOccupationIds.Contains(id);
        public void OpenCrafting(NPCController npc, GameObject player) => Debug.Log($"Open crafting for {npc.identity.displayName}");
    }

    /// <summary>Development adapter showing occupation-gated alchemy.</summary>
    public class ExampleOccupationAlchemyAdapter : MonoBehaviour, INPCSystemCapabilityProvider, INPCAlchemyProvider
    {
        public List<string> allowedOccupationIds = new() { "alchemist", "guild_alchemist", "healer" };
        public void GetCapabilities(List<NPCSystemCapability> c) => c.Add(NPCSystemCapability.Alchemy);
        public bool CanHandle(NPCSystemCapability c) => c == NPCSystemCapability.Alchemy;
        public bool CanOccupationUseAlchemy(string id) => allowedOccupationIds.Contains(id);
        public void OpenAlchemy(NPCController npc, GameObject player) => Debug.Log($"Open alchemy for {npc.identity.displayName}");
    }
}
