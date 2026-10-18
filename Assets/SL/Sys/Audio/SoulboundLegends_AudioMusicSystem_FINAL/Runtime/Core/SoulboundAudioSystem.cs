using System;
using System.Collections.Generic;
using UnityEngine;
namespace SoulboundLegends.AudioMusic
{
    [DisallowMultipleComponent]
    public sealed class SoulboundAudioSystem : MonoBehaviour
    {
        public static SoulboundAudioSystem Instance { get; private set; }
        [Header("Connections")] [SerializeField] List<MonoBehaviour> systems=new();
        [Header("Audio")] [SerializeField] AudioProfile defaultProfile;
        [SerializeField] AudioClip silentFallback;
        [SerializeField,Range(4,128)] int poolSize=32;
        [SerializeField,Range(1,256)] int maxSimultaneous=64;
        [SerializeField] Transform audioRoot;
        [Header("Mobile")] [SerializeField] AudioQuality quality=AudioQuality.High;
        [SerializeField] bool distanceCulling=true;
        readonly List<AudioSource> pool=new();
        readonly Dictionary<AudioSource,float> busyUntil=new();
        readonly Dictionary<string,AudioProfile> profiles=new();
        public event Action<AudioRequest> SoundPlayed;
        public IReadOnlyList<MonoBehaviour> Connections => systems;
        void Awake(){ if(Instance&&Instance!=this){Destroy(gameObject);return;} Instance=this; DontDestroyOnLoad(gameObject); BuildPool(); RegisterConnections(); }
        void BuildPool(){ if(!audioRoot){var go=new GameObject("AudioSources");go.transform.SetParent(transform);audioRoot=go.transform;} for(int i=0;i<poolSize;i++)CreateSource(); }
        AudioSource CreateSource(){var go=new GameObject("PooledAudioSource");go.transform.SetParent(audioRoot);var s=go.AddComponent<AudioSource>();s.playOnAwake=false;pool.Add(s);return s;}
        void RegisterConnections(){foreach(var mb in systems)if(mb is ISoulboundAudioCapabilityProvider p){if(p.TryGetAudioProfile(mb.name,out var profile)&&profile)profiles[profile.ProfileId]=profile;}}
        public void RegisterProfile(AudioProfile p){if(p&& !string.IsNullOrEmpty(p.ProfileId))profiles[p.ProfileId]=p;}
        public void Play(AudioRequest request, AudioProfile profile=null)
        {
            if(pool.Count==0)return;
            if(request.Category==AudioCategory.Music) return;
            if(!TryAcquire(request.Priority,out var source))return;
            profile ??= defaultProfile;
            var entry=profile? .Find(request.SoundType,request.Surface,request.Element):null;
            if(entry==null || entry.Clips==null || entry.Clips.Count==0){ if(silentFallback){source.clip=silentFallback;source.volume=0;}else{source.Stop();return;} }
            else{
                source.clip=entry.Clips[UnityEngine.Random.Range(0,entry.Clips.Count)];
                source.volume=Mathf.Clamp01(entry.Volume*request.VolumeMultiplier);
                source.pitch=Mathf.Clamp(entry.PitchMultiplier*UnityEngine.Random.Range(entry.PitchMin,entry.PitchMax),0.5f,2f);
                source.spatialBlend=entry.ThreeD?1f:0f; source.minDistance=entry.MinDistance;source.maxDistance=entry.MaxDistance;
                source.rolloffMode=AudioRolloffMode.Logarithmic; source.priority=256-Mathf.Clamp(request.Priority,0,256);
                source.transform.position=request.Position; source.loop=entry.Loop||request.Loop; source.Play();
            }
            busyUntil[source]=Time.unscaledTime+(source.clip?source.clip.length:0.05f); SoundPlayed?.Invoke(request);
        }
        bool TryAcquire(int priority,out AudioSource source){
            source=null; AudioSource free=null; int worst=-1; AudioSource worstSource=null;
            foreach(var s in pool){if(!s.isPlaying){free=s;break;} int p=256-s.priority;if(p<worst||worst<0){worst=p;worstSource=s;}}
            source=free??(worstSource&&priority>worst?worstSource:null); return source;
        }
        public void PlayFootstep(Vector3 position,MovementState movement,SurfaceType surface){var type=movement switch{MovementState.Walk=>SoundType.Walk,MovementState.Run=>SoundType.Run,MovementState.Sprint=>SoundType.Sprint,MovementState.Crouch=>SoundType.Footstep,_=>SoundType.Footstep};Play(CreateWithSurface(type,position,surface));}
        AudioRequest CreateWithSurface(SoundType type,Vector3 pos,SurfaceType surface){var r=AudioRequest.Create(type,AudioCategory.SFX,pos,60);r.Surface=surface;return r;}\n        public void Request(AudioRequest request){Play(request);}
    }
}
