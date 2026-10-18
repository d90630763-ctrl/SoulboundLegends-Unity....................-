using UnityEngine;
using UnityEngine.UI;

namespace SoulboundLegends.Progression
{
    public class SBLStatsPanel : MonoBehaviour
    {
        [Header("Optional UI References")]
        [SerializeField] private GameObject panel;
        [SerializeField] private Text title;
        [SerializeField] private Text levelText;
        [SerializeField] private Text statsText;
        [SerializeField] private Text ranksText;
        [SerializeField] private Text classText;
        [SerializeField] private Text powerText;

        void OnEnable()
        {
            if (SBLProgressionService.Instance != null)
                SBLProgressionService.Instance.OnProgressionChanged += Refresh;
            Refresh();
        }

        void OnDisable()
        {
            if (SBLProgressionService.Instance != null)
                SBLProgressionService.Instance.OnProgressionChanged -= Refresh;
        }

        public void Toggle()
        {
            if (panel == null) panel = gameObject;
            panel.SetActive(!panel.activeSelf);
            if (panel.activeSelf) Refresh();
        }

        public void Show() { if (panel != null) panel.SetActive(true); Refresh(); }
        public void Hide() { if (panel != null) panel.SetActive(false); }

        public void Refresh()
        {
            var s = SBLProgressionService.Instance;
            if (s == null) return;
            var d = s.Data;
            var st = d.totalStats;

            if (levelText) levelText.text = $"Adventure LV {d.adventureLevel} / {d.adventureLevelCap}";
            if (statsText) statsText.text =
                $"HP  {st.hp}\nMP  {st.mp}\nAttack  {st.attack}\nDefense  {st.defense}\n" +
                $"Magic Attack  {st.magicAttack}\nMagic Defense  {st.magicDefense}\n" +
                $"Attack Speed  {st.attackSpeed}\nMovement Speed  {st.movementSpeed}\n" +
                $"Critical  {st.critical}\nStamina  {st.stamina}\nSkill Power  {st.skillPower}";
            if (ranksText) ranksText.text =
                $"CS  {d.csRank.tier} ({d.csRank.rating})\n" +
                $"BR  {d.brRank.tier} ({d.brRank.rating})\n" +
                $"Siege  {d.siegeRank.tier} ({d.siegeRank.rating})";
            if (classText) classText.text = $"Class: {d.evaluatedClass}  ({d.classConfidence:0}% confidence)";
            if (powerText) powerText.text = $"Power: {d.totalPower}";
        }
    }
}
