using UnityEngine;

namespace SoulboundLegends.Lobby
{
    public class SL_CompatibilityChecker : MonoBehaviour
    {
        public SL_LobbySystem lobby;
        public SL_SystemIntegrationManager integration;
        public SL_ServerAuthorityBridge server;
        public bool runOnStart = true;
        public bool showWarnings = true;
        public bool blockGameIfCritical = false;

        private void Start() { if (runOnStart) RunCheck(); }

        public bool RunCheck()
        {
            bool ok = true;
            if (lobby == null) { ok = false; Warn("Lobby controller missing."); }
            if (integration == null) { ok = false; Warn("System Integration Manager missing."); }
            if (server == null) Warn("Server bridge missing; authoritative online validation will not be available.");
            if (blockGameIfCritical && !ok) { enabled = false; }
            return ok;
        }

        private void Warn(string message) { if (showWarnings) Debug.LogWarning("Soulbound Legends Compatibility: " + message); }
    }
}
