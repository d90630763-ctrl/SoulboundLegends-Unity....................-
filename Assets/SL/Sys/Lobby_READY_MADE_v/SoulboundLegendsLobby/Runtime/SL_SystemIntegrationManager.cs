using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.Lobby
{
    public class SL_SystemIntegrationManager : MonoBehaviour
    {
        public List<SL_SystemDefinition> systems = new List<SL_SystemDefinition>();
        public SL_ServerAuthorityBridge serverBridge;
        public bool initializeOnStart = true;
        public bool showWarnings = true;

        private readonly Dictionary<string, SL_SystemDefinition> lookup = new Dictionary<string, SL_SystemDefinition>();

        private void Awake()
        {
            RebuildLookup();
            if (initializeOnStart) StartCoroutine(InitializeSystems());
        }

        public void RebuildLookup()
        {
            lookup.Clear();
            foreach (var s in systems)
                if (s != null && !string.IsNullOrEmpty(s.systemID)) lookup[s.systemID] = s;
        }

        public bool TryGet(string systemID, out SL_SystemDefinition definition) => lookup.TryGetValue(systemID, out definition);

        public IEnumerator InitializeSystems()
        {
            foreach (var s in systems)
            {
                if (s == null || !s.enabled || !s.initializeOnStartup) continue;
                if (s.controller != null) s.onSystemReady?.Invoke();
                else { if (showWarnings) Debug.LogWarning("Soulbound Legends Lobby: missing controller for system " + s.systemID); s.onSystemError?.Invoke(); }
                yield return null;
            }
        }
    }
}
