using UnityEngine;

namespace SoulboundLegends.Progression
{
    public class SBLStatsButtonBridge : MonoBehaviour
    {
        [SerializeField] private SBLStatsPanel targetPanel;

        public void OpenStats()
        {
            if (targetPanel != null) targetPanel.Show();
        }

        public void CloseStats()
        {
            if (targetPanel != null) targetPanel.Hide();
        }

        public void ToggleStats()
        {
            if (targetPanel != null) targetPanel.Toggle();
        }
    }
}
