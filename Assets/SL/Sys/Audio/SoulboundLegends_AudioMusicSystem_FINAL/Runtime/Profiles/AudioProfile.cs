using System.Collections.Generic;
using UnityEngine;
namespace SoulboundLegends.AudioMusic
{
    [CreateAssetMenu(menuName="Soulbound Legends/Audio/Audio Profile")]
    public sealed class AudioProfile : ScriptableObject
    {
        public string ProfileId;
        public AudioCategory DefaultCategory = AudioCategory.SFX;
        public List<SoundEntry> Sounds = new();
        public SoundEntry Find(SoundType type, SurfaceType surface=SurfaceType.Auto, ElementType element=ElementType.None)
        {
            SoundEntry fallback=null;
            foreach(var s in Sounds) {
                if(s.SoundType!=type || s.Clips==null || s.Clips.Count==0) continue;
                if(s.Surface==surface && (s.Element==element || s.Element==ElementType.None)) return s;
                if(s.Surface==SurfaceType.Auto && (s.Element==element || s.Element==ElementType.None)) fallback=s;
                if(fallback==null) fallback=s;
            }
            return fallback;
        }
    }
    [System.Serializable]
    public sealed class SoundEntry
    {
        public string Name;
        public SoundType SoundType;
        public SurfaceType Surface=SurfaceType.Auto;
        public ElementType Element=ElementType.None;
        public List<AudioClip> Clips=new();
        [Range(0,1)] public float Volume=1;
        [Range(0.5f,2)] public float PitchMin=0.95f;
        [Range(0.5f,2)] public float PitchMax=1.05f;
        [Range(0,256)] public int Priority=128;
        public bool Loop;
        public bool ThreeD=true;
        [Min(0.01f)] public float MinDistance=1;
        [Min(1)] public float MaxDistance=40;
        public float Rolloff=1;
    }
}
