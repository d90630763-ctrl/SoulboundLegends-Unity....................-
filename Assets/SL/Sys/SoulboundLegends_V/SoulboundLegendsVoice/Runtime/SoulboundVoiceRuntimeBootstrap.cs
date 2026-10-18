using UnityEngine;

namespace SoulboundLegends.Voice
{
    public static class SoulboundVoiceRuntimeBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateIfMissing()
        {
            if (Object.FindObjectOfType<VoiceTransformationSystem>() != null)
                return;

            GameObject go = new GameObject("Soulbound Legends Voice Transformation System");
            go.AddComponent<VoiceTransformationSystem>();
            Object.DontDestroyOnLoad(go);
        }
    }
}
