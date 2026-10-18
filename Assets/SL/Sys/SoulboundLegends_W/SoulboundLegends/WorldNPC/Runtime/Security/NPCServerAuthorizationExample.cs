using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.WorldNPC
{
    public class NPCServerAuthorizationAdapter : MonoBehaviour, INPCSystemCapabilityProvider, INPCServerAuthorizationProvider
    {
        public void GetCapabilities(List<NPCSystemCapability> capabilities) => capabilities.Add(NPCSystemCapability.ServerAuthorization);
        public bool CanHandle(NPCSystemCapability capability) => capability == NPCSystemCapability.ServerAuthorization;
        public bool ValidateInteraction(GameObject player, NPCController npc, NPCServiceType service, out string reason) { reason = "Approved by local adapter; replace with authoritative server implementation."; return true; }
        public bool ValidateAction(GameObject player, NPCController npc, string actionId, out string reason) { reason = "Approved by local adapter; replace with authoritative server implementation."; return true; }
    }
}
