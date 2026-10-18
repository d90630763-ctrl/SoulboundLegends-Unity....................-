using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace SoulboundLegends.SkillVFX
{
    public enum SkillVFXElement { None, Fire, Water, Earth, Air, Lightning, Ice, Nature, Light, Void, Shadow }
    public enum SkillVFXForm { None, Slash, Projectile, Beam, Explosion, Shockwave, Aura, Sphere, Cone, GroundEruption, Wave, Spiral, WeaponEnhancement }
    public enum SkillVFXQuality { Low, Medium, High, Ultra }

    [Serializable]
    public class SkillVFXDescriptor
    {
        public string skillId;
        public string skillName;
        public SkillVFXElement element;
        public SkillVFXForm form;
        [Min(0)] public float damage;
        [Min(0)] public float range = 5f;
        [Min(0)] public float areaRadius = 1f;
        [Min(0)] public float duration = 1f;
        [Min(0)] public float projectileSpeed = 10f;
        public bool projectile;
        public bool trail;
        public bool explosionOnImpact;
        public bool groundEffect;
        public bool chargeEffect;
        [Min(1)] public int skillLevel = 1;
        public Color customColor = Color.white;
    }

    [Serializable]
    public class SkillVFXRule
    {
        public SkillVFXElement element;
        public SkillVFXForm form;
        public VisualEffectAsset template;
        [Min(1)] public int minimumSkillLevel = 1;
        public bool useForProjectile;
        public bool useForImpact;
        public bool useForTrail;
    }

    public sealed class SkillVFXGenerationSystem : MonoBehaviour
    {
        [Header("Generic Connections")]
        [SerializeField] private MonoBehaviour skillSystem;
        [SerializeField] private MonoBehaviour environmentSystem;

        [Header("VFX Rules")]
        [SerializeField] private List<SkillVFXRule> rules = new List<SkillVFXRule>();

        [Header("Runtime")]
        [SerializeField] private Transform vfxRoot;
        [SerializeField] private SkillVFXQuality quality = SkillVFXQuality.High;
        [SerializeField] private int maxLiveEffects = 80;
        [SerializeField] private bool automaticParameterScaling = true;
        [SerializeField] private bool usePooling = true;

        private readonly Queue<VisualEffect> pool = new Queue<VisualEffect>();
        private readonly List<VisualEffect> live = new List<VisualEffect>();

        public void SetSkillSystem(MonoBehaviour system) => skillSystem = system;
        public void SetEnvironmentSystem(MonoBehaviour system) => environmentSystem = system;

        public void PlaySkillVFX(SkillVFXDescriptor skill, Vector3 position, Quaternion rotation)
        {
            Play(skill, position, rotation, false);
        }

        public void PlayImpactVFX(SkillVFXDescriptor skill, Vector3 position)
        {
            Play(skill, position, Quaternion.identity, true);
        }

        public void StopAllVFX()
        {
            for (int i = live.Count - 1; i >= 0; i--)
                Release(live[i]);
            live.Clear();
        }

        private void Play(SkillVFXDescriptor skill, Vector3 position, Quaternion rotation, bool impact)
        {
            if (skill == null || live.Count >= maxLiveEffects) return;
            VisualEffectAsset asset = FindTemplate(skill, impact);
            if (asset == null) return;

            VisualEffect effect = GetEffect(asset);
            if (effect == null) return;

            effect.transform.SetPositionAndRotation(position, rotation);
            ApplyParameters(effect, skill);
            effect.gameObject.SetActive(true);
            effect.Play();
            live.Add(effect);
        }

        private VisualEffectAsset FindTemplate(SkillVFXDescriptor skill, bool impact)
        {
            SkillVFXRule fallback = null;
            foreach (var rule in rules)
            {
                if (rule == null || rule.template == null) continue;
                if (skill.skillLevel < rule.minimumSkillLevel) continue;
                if (rule.element != SkillVFXElement.None && rule.element != skill.element) continue;
                if (rule.form != SkillVFXForm.None && rule.form != skill.form) continue;

                if (impact && rule.useForImpact) return rule.template;
                if (!impact) fallback = rule;
            }
            return fallback != null ? fallback.template : null;
        }

        private VisualEffect GetEffect(VisualEffectAsset asset)
        {
            if (usePooling && pool.Count > 0)
            {
                int count = pool.Count;
                while (count-- > 0)
                {
                    var e = pool.Dequeue();
                    if (e != null && e.visualEffectAsset == asset) return e;
                    if (e != null) pool.Enqueue(e);
                }
            }

            GameObject go = new GameObject("SkillVFX_" + asset.name);
            if (vfxRoot != null) go.transform.SetParent(vfxRoot, false);
            var effect = go.AddComponent<VisualEffect>();
            effect.visualEffectAsset = asset;
            return effect;
        }

        private void ApplyParameters(VisualEffect effect, SkillVFXDescriptor skill)
        {
            float q = quality == SkillVFXQuality.Low ? .5f :
                      quality == SkillVFXQuality.Medium ? .75f :
                      quality == SkillVFXQuality.Ultra ? 1.35f : 1f;

            if (!automaticParameterScaling) q = 1f;

            if (effect.HasFloat("SkillLevel")) effect.SetFloat("SkillLevel", skill.skillLevel);
            if (effect.HasFloat("Damage")) effect.SetFloat("Damage", skill.damage);
            if (effect.HasFloat("Range")) effect.SetFloat("Range", skill.range);
            if (effect.HasFloat("Radius")) effect.SetFloat("Radius", skill.areaRadius * q);
            if (effect.HasFloat("Duration")) effect.SetFloat("Duration", skill.duration);
            if (effect.HasFloat("ProjectileSpeed")) effect.SetFloat("ProjectileSpeed", skill.projectileSpeed);
            if (effect.HasVector4("Color")) effect.SetVector4("Color", skill.customColor);
            if (effect.HasFloat("Intensity"))
                effect.SetFloat("Intensity", Mathf.Clamp(skill.damage / 100f, .25f, 10f) * q);
        }

        private void Release(VisualEffect effect)
        {
            if (effect == null) return;
            effect.Stop();
            effect.gameObject.SetActive(false);
            if (usePooling) pool.Enqueue(effect);
            else Destroy(effect.gameObject);
        }
    }
}
