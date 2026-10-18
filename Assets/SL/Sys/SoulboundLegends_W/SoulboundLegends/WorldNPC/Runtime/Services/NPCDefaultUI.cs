using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundLegends.WorldNPC
{
    /// <summary>Small dependency-free fallback UI. A project's UI Generation System can replace this through INPCUIProvider.</summary>
    public class NPCDefaultUI : MonoBehaviour, INPCSystemCapabilityProvider, INPCUIProvider
    {
        public Canvas canvas;
        public bool createIfMissing = true;
        public Text titleText;
        public Text dialogueText;
        private GameObject panel;

        public void GetCapabilities(List<NPCSystemCapability> c) => c.Add(NPCSystemCapability.UI);
        public bool CanHandle(NPCSystemCapability c) => c == NPCSystemCapability.UI;

        public void OpenNPCUI(NPCController npc, GameObject player)
        {
            if (!npc) return;
            if (!canvas && createIfMissing) CreateUI();
            if (!canvas) return;
            canvas.gameObject.SetActive(true);
            if (titleText) titleText.text = npc.identity.displayName + (string.IsNullOrEmpty(npc.identity.title) ? "" : " — " + npc.identity.title);
            if (dialogueText) dialogueText.text = npc.identity.occupationId;
        }

        private void CreateUI()
        {
            var go = new GameObject("NPC_Runtime_UI", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            go.transform.SetParent(transform, false);
            canvas = go.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1920,1080);
            panel = new GameObject("NPCPanel", typeof(Image)); panel.transform.SetParent(go.transform, false);
            var img = panel.GetComponent<Image>(); img.rectTransform.anchorMin = new Vector2(.2f,.15f); img.rectTransform.anchorMax = new Vector2(.8f,.45f); img.rectTransform.offsetMin = img.rectTransform.offsetMax = Vector2.zero;
            titleText = MakeText(panel.transform, "NPCName", new Vector2(0.05f,.55f), new Vector2(.95f,.9f), 32);
            dialogueText = MakeText(panel.transform, "NPCInfo", new Vector2(.05f,.15f), new Vector2(.95f,.5f), 22);
            var close = new GameObject("Close", typeof(Button)); close.transform.SetParent(panel.transform, false);
            var cb = close.GetComponent<Button>(); close.GetComponent<RectTransform>().anchorMin = new Vector2(.82f,.02f); close.GetComponent<RectTransform>().anchorMax = new Vector2(.98f,.16f); close.GetComponent<RectTransform>().offsetMin = close.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            var t = MakeText(close.transform, "CloseText", Vector2.zero, Vector2.one, 20); t.text = "Close";
            cb.onClick.AddListener(() => canvas.gameObject.SetActive(false));
            canvas.gameObject.SetActive(false);
        }

        private static Text MakeText(Transform parent, string name, Vector2 min, Vector2 max, int size)
        {
            var go = new GameObject(name, typeof(Text)); go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>(); rt.anchorMin = min; rt.anchorMax = max; rt.offsetMin = rt.offsetMax = Vector2.zero;
            var text = go.GetComponent<Text>(); text.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); text.fontSize = size; text.alignment = TextAnchor.MiddleCenter; text.color = Color.white;
            return text;
        }
    }

    public class NPCProximityTrigger : MonoBehaviour
    {
        public NPCInteractionController interaction;
        public string playerTag = "Player";
        public bool useTriggerCollider = true;
        private void Awake()
        {
            if (!interaction) interaction = GetComponent<NPCInteractionController>();
            if (useTriggerCollider)
            {
                var c = GetComponent<SphereCollider>();
                if (!c) c = gameObject.AddComponent<SphereCollider>();
                c.isTrigger = true; c.radius = interaction ? interaction.detectionDistance : 3f;
            }
        }
        private void OnTriggerEnter(Collider other) { if (other.CompareTag(playerTag) && interaction) interaction.Approach(other.gameObject); }
        private void OnTriggerExit(Collider other) { if (other.CompareTag(playerTag) && interaction) interaction.Leave(other.gameObject); }
    }
}
