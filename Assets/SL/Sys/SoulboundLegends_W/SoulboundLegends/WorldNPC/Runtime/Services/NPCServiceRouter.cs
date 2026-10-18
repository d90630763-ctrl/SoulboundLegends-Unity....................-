using UnityEngine;

namespace SoulboundLegends.WorldNPC
{
    public class NPCServiceRouter : MonoBehaviour
    {
        public NPCController npc;
        private void Awake() { if (!npc) npc = GetComponent<NPCController>(); }

        public bool TryUseService(GameObject player, NPCServiceType service)
        {
            if (!npc || !player || !npc.HasService(service)) return false;
            var auth = npc.registry ? npc.registry.Find<INPCServerAuthorizationProvider>() : null;
            if (auth != null && !auth.ValidateInteraction(player, npc, service, out _)) return false;

            switch (service)
            {
                case NPCServiceType.Quest: { var q = npc.registry?.Find<INPCQuestProvider>(); if (q == null) return false; q.OpenQuestInterface(npc, player); return true; }
                case NPCServiceType.Craft: return OpenCraft(player);
                case NPCServiceType.Alchemy: return OpenAlchemy(player);
                case NPCServiceType.Shop: return OpenShop(player);
                case NPCServiceType.Talk: NPCDialogueRuntime.SpeakApproach(npc, player); return true;
                default: return true;
            }
        }

        private bool OpenCraft(GameObject player)
        {
            var provider = npc.registry?.Find<INPCCraftingProvider>();
            if (provider == null || !provider.CanOccupationCraft(npc.identity.occupationId)) return false;
            provider.OpenCrafting(npc, player); return true;
        }
        private bool OpenAlchemy(GameObject player)
        {
            var provider = npc.registry?.Find<INPCAlchemyProvider>();
            if (provider == null || !provider.CanOccupationUseAlchemy(npc.identity.occupationId)) return false;
            provider.OpenAlchemy(npc, player); return true;
        }
        private bool OpenShop(GameObject player) { var p = npc.registry?.Find<INPCShopProvider>(); if (p == null) return false; p.OpenShop(npc, player); return true; }
    }

}
