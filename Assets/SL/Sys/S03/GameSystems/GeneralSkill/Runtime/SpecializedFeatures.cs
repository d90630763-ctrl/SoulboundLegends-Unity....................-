using System;
using UnityEngine;
namespace GameSystems.GeneralSkill {
    public enum SkillType { Attack, Healing, Movement, Utility }
    public enum SkillElement { Physical, Fire, Water, Earth, Void, Thunder, Nature, Light, Air, Ice, Omni, Shadow }
    [CreateAssetMenu(fileName="SkillDefinition",menuName="Game Systems/General Skill/Skill Definition")]
    public sealed class SkillDefinition : ScriptableObject {
        public string skillId="skill_001"; public string displayName="Skill"; [TextArea] public string description;
        public SkillType type; public SkillElement element; public AnimationClip animation; public Sprite icon;
        public float range=5, areaRadius=1, damage=10, healing=10, resourceCost=5, cooldown=1;
        public int skillLevel=1, skillLevelCap=100; public float perLevelScale=0.1f;
        public string[] forms, buffs, debuffs; public bool canDestroyTerrain; public GameObject vfxPrefab; public AudioClip sfx;
        public float GetScaledValue(float baseValue,int attackerLevel,int targetLevel) {
            float levelFactor=1f+Mathf.Max(0,attackerLevel-skillLevel)*perLevelScale;
            if(targetLevel>attackerLevel) levelFactor/=1f+(targetLevel-attackerLevel)*0.05f;
            return baseValue*levelFactor;
        }
    }
}