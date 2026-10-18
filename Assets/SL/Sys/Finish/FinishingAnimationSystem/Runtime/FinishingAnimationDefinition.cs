using System;
using UnityEngine;

namespace SoulboundLegends.Finishing
{
    public enum FinisherTargetRule
    {
        EnemyOnly,
        AnyEligibleTarget
    }

    [CreateAssetMenu(fileName = "FinishingAnimation", menuName = "Soulbound Legends/Combat/Finishing Animation")]
    public class FinishingAnimationDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string finisherId;
        public string displayName;
        [TextArea] public string description;

        [Header("Animation")]
        public AnimationClip finisherAnimation;
        public AvatarMask avatarMask;
        [Min(0.01f)] public float duration = 2.5f;
        public string animatorTrigger = "Finisher";

        [Header("Eligibility")]
        [Range(0.01f, 100f)] public float maximumTargetHealthPercent = 1f;
        [Min(0.1f)] public float range = 3f;
        public bool requireLineOfSight = true;
        public FinisherTargetRule targetRule = FinisherTargetRule.EnemyOnly;
        public bool allowedInCombat = true;

        [Header("Use")]
        [Min(0f)] public float cooldownSeconds = 20f;
        public bool requiresOwnership = true;
        public bool usableAsSkill = true;

        [Header("Shop")]
        public bool purchasable = false;
        public string shopProductId;
        public int price = 0;

        [Header("Optional Skill Integration")]
        public string skillId;
        public bool skillCanTriggerFinisher = false;

        [Header("Presentation Hooks")]
        public GameObject vfxPrefab;
        public AudioClip startSound;
        public AudioClip impactSound;
        public AudioClip finishSound;
    }
}
