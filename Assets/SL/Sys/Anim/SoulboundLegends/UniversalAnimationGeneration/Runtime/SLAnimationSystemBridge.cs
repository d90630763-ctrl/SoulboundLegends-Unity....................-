using System.Collections.Generic;
using UnityEngine;
namespace SoulboundLegends.Animation
{
    public interface ISLAnimationSystemListener { void OnAnimationVFX(string id,GameObject actor); void OnAnimationAudio(string id,GameObject actor); void OnAnimationGameplay(string id,GameObject actor); }
    [DisallowMultipleComponent] public sealed class SLAnimationSystemBridge:MonoBehaviour
    {
        [SerializeField] SLAnimationRuntime runtime; readonly List<ISLAnimationSystemListener> listeners=new();
        public void Register(ISLAnimationSystemListener l){if(l!=null&&!listeners.Contains(l))listeners.Add(l);} public void Unregister(ISLAnimationSystemListener l)=>listeners.Remove(l);
        void Awake(){if(!runtime)runtime=GetComponent<SLAnimationRuntime>();if(!runtime)return;runtime.VFXRequested+=VFX;runtime.AudioRequested+=Audio;runtime.GameplayRequested+=Gameplay;}
        void OnDestroy(){if(!runtime)return;runtime.VFXRequested-=VFX;runtime.AudioRequested-=Audio;runtime.GameplayRequested-=Gameplay;}
        void VFX(string id){foreach(var l in listeners)l.OnAnimationVFX(id,gameObject);} void Audio(string id){foreach(var l in listeners)l.OnAnimationAudio(id,gameObject);} void Gameplay(string id){foreach(var l in listeners)l.OnAnimationGameplay(id,gameObject);}
    }
}
