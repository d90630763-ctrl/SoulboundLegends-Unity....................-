using System;
using UnityEngine;
namespace SoulboundLegends.AudioMusic
{
    public sealed class AudioEventRouter : MonoBehaviour
    {
        [SerializeField] SoulboundAudioSystem audioSystem;
        public event Action<AudioRequest> EventRaised;
        void Awake(){audioSystem??=GetComponent<SoulboundAudioSystem>();}
        public void Raise(AudioRequest request){EventRaised?.Invoke(request);audioSystem?.Request(request);}
        public void RaiseAt(SoundType type,AudioCategory category,Vector3 position,int priority=128,float intensity=1f){
            var r=AudioRequest.Create(type,category,position,priority);r.Intensity=intensity;Raise(r);
        }
        public void RaiseServerConfirmed(AuthoritativeAudioEvent e){RaiseAt(e.SoundType,e.Category,e.Position,e.Priority,e.Intensity);}
    }
}
