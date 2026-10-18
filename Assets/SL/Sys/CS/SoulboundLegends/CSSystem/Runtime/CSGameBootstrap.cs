using UnityEngine;

namespace SoulboundLegends.CS
{
    public class CSGameBootstrap : MonoBehaviour
    {
        public CSWorldSystem world;
        public bool autoValidateOnStart = true;

        private void Start()
        {
            if (!autoValidateOnStart || world == null) return;
            if (!world.ValidateConfiguration(out var errors))
            {
                foreach (var e in errors) Debug.LogError("[Soulbound Legends CS] " + e, this);
            }
        }
    }
}
