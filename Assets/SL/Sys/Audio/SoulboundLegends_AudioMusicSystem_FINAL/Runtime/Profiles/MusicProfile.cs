using System;
using System.Collections.Generic;
using UnityEngine;
namespace SoulboundLegends.AudioMusic
{
    [CreateAssetMenu(menuName="Soulbound Legends/Audio/Music Profile")]
    public sealed class MusicProfile : ScriptableObject
    {
        public List<MusicEntry> Entries=new();
        public MusicEntry Find(MusicContext context, string region="")
        {
            MusicEntry best=null;
            foreach(var e in Entries) if(e.Clip && e.Context==context && (string.IsNullOrEmpty(e.Region)||e.Region==region))
                if(best==null || e.Priority>best.Priority) best=e;
            return best;
        }
    }
    [Serializable]
    public sealed class MusicEntry
    {
        public string Name;
        public AudioClip Clip;
        public MusicContext Context;
        public string Region;
        [Range(0,1)] public float Volume=1;
        [Range(0,256)] public int Priority=100;
        public bool Loop=true;
        public bool RandomEligible=true;
        [Min(0)] public float Weight=1;
        public MusicTransitionMode Transition=MusicTransitionMode.Crossfade;
        [Min(0)] public float TransitionSeconds=1.5f;
        public bool ResumePrevious=true;
        public bool RestartOnEnter=false;
    }
}
