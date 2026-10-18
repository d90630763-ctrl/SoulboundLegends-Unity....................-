using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.Animation
{
    public enum SLAnimationActionType { Idle, Step, Walk, Run, Sprint, Turn, LookAt, Reach, Grab, Pull, Push, DrawWeapon, SheathWeapon, Slash, HeavySlash, Block, Dodge, Jump, Fall, Land, Crouch, Stand, Attack, HitReact, Death, Emote, HoldPose, Custom }

    [Serializable]
    public class SLAnimationAction
    {
        public string id = "Action";
        public SLAnimationActionType type = SLAnimationActionType.Idle;
        [TextArea(1,3)] public string instruction;
        [Min(0.05f)] public float duration = .5f;
        [Range(0,1)] public float blendIn = .08f;
        [Range(0,1)] public float blendOut = .08f;
        [Range(0,2)] public float intensity = 1;
        public float forwardDistance;
        public float turnDegrees;
        public string markerOnStart;
        public string markerOnEnd;
        public string vfxEventId;
        public string audioEventId;
        public string gameplayEventId;
    }

    [Serializable]
    public class SLAnimationMarker
    {
        public string id;
        [Min(0)] public float time;
        public string vfxEventId;
        public string audioEventId;
        public string gameplayEventId;
    }

    [CreateAssetMenu(menuName="Soulbound Legends/Animation/Animation Definition", fileName="SL_AnimationDefinition")]
    public class SLAnimationDefinition : ScriptableObject
    {
        public string animationId = "Animation_001";
        public string displayName = "New Animation";
        [TextArea(4,20)] public string description;
        public bool humanoid = true;
        public bool universalRetargetable = true;
        public bool useRootMotion = true;
        public bool useIK = true;
        public List<SLAnimationAction> actions = new();
        public List<SLAnimationMarker> markers = new();
    }

    [Serializable]
    public class SLAnimationSequence
    {
        public string sequenceId;
        public List<SLAnimationAction> actions = new();
        public List<SLAnimationMarker> markers = new();
        public float duration;
        public void RecalculateDuration(){ duration=0; foreach(var a in actions) if(a!=null) duration+=Mathf.Max(.05f,a.duration); }
    }
}
