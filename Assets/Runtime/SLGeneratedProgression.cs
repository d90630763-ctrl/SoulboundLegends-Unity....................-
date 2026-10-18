using UnityEngine;
using UnityEngine.UI;

namespace SoulboundLegends.Runtime
{
    /// <summary>Local test progression used by Builder-generated quest/XP HUD. Production accounts must validate progression on the server.</summary>
    public sealed class SLGeneratedProgression : MonoBehaviour
    {
        public int level = 1;
        public int experience;
        public int maxLevel = 100;
        public int experienceToNextLevel = 100;
        public int health = 1000;
        Text statsText;
        Text levelUpText;
        float nextRefresh;

        void Start()
        {
            level = Mathf.Clamp(PlayerPrefs.GetInt("SL_PlayerLevel", level), 1, maxLevel);
            experience = Mathf.Max(0, PlayerPrefs.GetInt("SL_PlayerXP", experience));
            experienceToNextLevel = Mathf.Max(100, PlayerPrefs.GetInt("SL_PlayerXPNext", experienceToNextLevel));
            var stats = GameObject.Find("StatsHUD"); if (stats) statsText = stats.GetComponent<Text>();
            var levelUp = GameObject.Find("LevelUpNotice"); if (levelUp) { levelUpText = levelUp.GetComponent<Text>(); levelUp.SetActive(false); }
            RefreshHud();
        }
        void Update()
        {
            if (Time.unscaledTime >= nextRefresh) { nextRefresh = Time.unscaledTime + .25f; RefreshHud(); }
        }
        public void AddExperience(int amount)
        {
            if (amount <= 0 || level >= maxLevel) return;
            experience += amount;
            bool gained = false;
            while (level < maxLevel && experience >= experienceToNextLevel)
            {
                experience -= experienceToNextLevel; level++; gained = true;
                experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.12f);
            }
            PlayerPrefs.SetInt("SL_PlayerLevel", level);
            PlayerPrefs.SetInt("SL_PlayerXP", experience);
            PlayerPrefs.SetInt("SL_PlayerXPNext", experienceToNextLevel);
            PlayerPrefs.Save();
            if (gained && levelUpText != null)
            {
                levelUpText.text = "LEVEL UP!  LV " + level;
                levelUpText.gameObject.SetActive(true);
                CancelInvoke(nameof(HideLevelUp)); Invoke(nameof(HideLevelUp), 2.5f);
            }
            RefreshHud();
        }
        public void GrantQuestReward() => AddExperience(50);
        void HideLevelUp() { if (levelUpText) levelUpText.gameObject.SetActive(false); }
        void RefreshHud()
        {
            if (statsText != null) statsText.text = "HP " + health + "     LV " + level + "     XP " + experience + " / " + experienceToNextLevel;
        }
    }
}
