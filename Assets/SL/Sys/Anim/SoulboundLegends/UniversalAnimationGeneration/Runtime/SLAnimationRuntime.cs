using System;
using UnityEngine;
namespace SoulboundLegends.Animation
{
    [DisallowMultipleComponent] public sealed class SLUniversalAnimationActor:MonoBehaviour
    {
        [SerializeField] Animator animator; [SerializeField] Transform lookTarget,rightHandTarget,leftHandTarget;
        public Animator Animator=>animator; public Transform LookTarget=>lookTarget; public Transform RightHandTarget=>rightHandTarget; public Transform LeftHandTarget=>leftHandTarget;
        public void Initialize(){if(!animator)animator=GetComponentInChildren<Animator>();}
        public void SetLookTarget(Transform t)=>lookTarget=t; public void SetRightHandTarget(Transform t)=>rightHandTarget=t; public void SetLeftHandTarget(Transform t)=>leftHandTarget=t;
        void Reset()=>Initialize(); void Awake()=>Initialize();
    }
    [DisallowMultipleComponent] public sealed class SLAnimationIKDriver:MonoBehaviour
    {
        [SerializeField] SLUniversalAnimationActor actor; [Range(0,1)] public float lookWeight=.65f; [Range(0,1)] public float rightHandWeight; [Range(0,1)] public float leftHandWeight;
        void Reset()=>actor=GetComponent<SLUniversalAnimationActor>();
        void OnAnimatorIK(int layerIndex){if(!actor||!actor.Animator)return;var a=actor.Animator;if(actor.LookTarget&&lookWeight>0){a.SetLookAtWeight(lookWeight,.25f,.65f,.8f,.45f);a.SetLookAtPosition(actor.LookTarget.position);}if(actor.RightHandTarget&&rightHandWeight>0){a.SetIKPositionWeight(AvatarIKGoal.RightHand,rightHandWeight);a.SetIKRotationWeight(AvatarIKGoal.RightHand,rightHandWeight);a.SetIKPosition(AvatarIKGoal.RightHand,actor.RightHandTarget.position);a.SetIKRotation(AvatarIKGoal.RightHand,actor.RightHandTarget.rotation);}if(actor.LeftHandTarget&&leftHandWeight>0){a.SetIKPositionWeight(AvatarIKGoal.LeftHand,leftHandWeight);a.SetIKRotationWeight(AvatarIKGoal.LeftHand,leftHandWeight);a.SetIKPosition(AvatarIKGoal.LeftHand,actor.LeftHandTarget.position);a.SetIKRotation(AvatarIKGoal.LeftHand,actor.LeftHandTarget.rotation);}}
    }
    public sealed class SLAnimationRuntime:MonoBehaviour
    {
        public event Action<SLAnimationMarker> MarkerTriggered; public event Action<string> VFXRequested,AudioRequested,GameplayRequested;
        SLAnimationSequence sequence; float time; int markerIndex; public bool IsPlaying=>sequence!=null&&time<sequence.duration; public float Time=>time;
        public void Play(SLAnimationSequence s){sequence=s;sequence?.RecalculateDuration();time=0;markerIndex=0;}
        public void Stop(){sequence=null;time=0;}
        void Update(){if(!IsPlaying)return;time+=UnityEngine.Time.deltaTime;while(markerIndex<sequence.markers.Count&&sequence.markers[markerIndex].time<=time){var m=sequence.markers[markerIndex++];MarkerTriggered?.Invoke(m);if(!string.IsNullOrEmpty(m.vfxEventId))VFXRequested?.Invoke(m.vfxEventId);if(!string.IsNullOrEmpty(m.audioEventId))AudioRequested?.Invoke(m.audioEventId);if(!string.IsNullOrEmpty(m.gameplayEventId))GameplayRequested?.Invoke(m.gameplayEventId);}}
    }
}
