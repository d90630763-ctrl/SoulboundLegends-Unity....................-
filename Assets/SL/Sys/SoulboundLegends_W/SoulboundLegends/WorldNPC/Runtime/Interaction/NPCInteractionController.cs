using UnityEngine;

namespace SoulboundLegends.WorldNPC
{
    public class NPCInteractionController : MonoBehaviour
    {
        public NPCController npc;
        public bool requireApproachGreeting = true;
        public float detectionDistance = 3f;
        private GameObject currentPlayer;
        private float lastGreeting;

        private void Awake() { if (!npc) npc = GetComponent<NPCController>(); }

        public void Approach(GameObject player)
        {
            if (!npc || !player) return;
            currentPlayer = player;
            npc.FacePlayer(player.transform);
            npc.SetInteracting(true);
            if (requireApproachGreeting && Time.time >= lastGreeting + 2f)
            {
                lastGreeting = Time.time;
                NPCDialogueRuntime.SpeakApproach(npc, player);
            }
            OpenUI(player);
        }

        public void Leave(GameObject player)
        {
            if (player != currentPlayer) return;
            currentPlayer = null;
            npc.SetInteracting(false);
        }

        private void OpenUI(GameObject player)
        {
            var ui = npc.registry ? npc.registry.Find<INPCUIProvider>() : null;
            if (ui != null) ui.OpenNPCUI(npc, player);
        }
    }
}
