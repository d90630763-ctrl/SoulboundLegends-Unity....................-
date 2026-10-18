using UnityEngine;

namespace SoulboundLegends.Progression
{
    // Optional demo/testing hooks. Safe to remove after integration.
    public class SBLExampleHooks : MonoBehaviour
    {
        public void Add100XP() => SBLProgressionService.Instance?.AddAdventureXP(100);
        public void WinCS() => SBLProgressionService.Instance?.RecordCompetitiveMatch("CS", true);
        public void LoseCS() => SBLProgressionService.Instance?.RecordCompetitiveMatch("CS", false);
        public void WinBR() => SBLProgressionService.Instance?.RecordCompetitiveMatch("BR", true);
        public void WinSiege() => SBLProgressionService.Instance?.RecordCompetitiveMatch("Siege", true);
        public void ExpandLevelCap() => SBLProgressionService.Instance?.ExpandAdventureCap();
    }
}
