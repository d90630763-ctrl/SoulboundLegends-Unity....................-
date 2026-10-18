using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.WorldNPC
{
    public interface INPCSystemCapabilityProvider
    {
        void GetCapabilities(List<NPCSystemCapability> capabilities);
        bool CanHandle(NPCSystemCapability capability);
    }

    public interface INPCVoiceProvider
    {
        bool TryAssignVoice(NPCController npc, NPCGender gender, int uniqueSeed);
        void Speak(NPCController npc, string text);
    }

    public interface INPCDialogueProvider
    {
        string GetApproachLine(NPCController npc, NPCDialogueContext context);
    }

    public interface INPCQuestProvider
    {
        bool HasQuestFor(NPCController npc);
        void OpenQuestInterface(NPCController npc, GameObject player);
    }

    public interface INPCCraftingProvider
    {
        bool CanOccupationCraft(string occupationId);
        void OpenCrafting(NPCController npc, GameObject player);
    }

    public interface INPCAlchemyProvider
    {
        bool CanOccupationUseAlchemy(string occupationId);
        void OpenAlchemy(NPCController npc, GameObject player);
    }

    public interface INPCShopProvider
    {
        void OpenShop(NPCController npc, GameObject player);
    }

    public interface INPCNavigationProvider
    {
        void SetDestination(NPCController npc, Vector3 destination);
        void Stop(NPCController npc);
    }

    public interface INPCUIProvider
    {
        void OpenNPCUI(NPCController npc, GameObject player);
    }

    public interface INPCServerAuthorizationProvider
    {
        bool ValidateInteraction(GameObject player, NPCController npc, NPCServiceType service, out string reason);
        bool ValidateAction(GameObject player, NPCController npc, string actionId, out string reason);
    }
}
