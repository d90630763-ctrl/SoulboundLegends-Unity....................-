using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SoulboundLegends.AudioMusic
{
    [DisallowMultipleComponent]
    public sealed class SoulboundMusicManager : MonoBehaviour
    {
        [SerializeField] MusicProfile library;
        [SerializeField] AudioMixerBridge mixer;
        [SerializeField] List<MusicEntryRuntime> musicEntries=new();
        [SerializeField] AudioSource sourceA;
        [SerializeField] AudioSource sourceB;
        [SerializeField,Range(0,1)] float masterMusicVolume=1;
        AudioSource active; MusicEntryRuntime activeEntry; Coroutine transition;
        readonly Dictionary<MusicContext,List<MusicEntryRuntime>> byContext=new();
        void Awake(){ if(!sourceA)sourceA=CreateSource("Music A");if(!sourceB)sourceB=CreateSource("Music B");active=sourceA;BuildIndex(); }
        AudioSource CreateSource(string n){var s=new GameObject(n).AddComponent<AudioSource>();s.transform.SetParent(transform);s.playOnAwake=false;s.loop=true;return s;}
        void BuildIndex(){byContext.Clear();foreach(var e in musicEntries){if(e==null||!e.Clip)continue;if(!byContext.ContainsKey(e.Context))byContext[e.Context]=new();byContext[e.Context].Add(e);}}
        public void AddMusic(MusicEntryRuntime entry){if(entry==null)return;musicEntries.Add(entry);BuildIndex();}
        public void PlayContext(MusicContext context,string region="",bool force=false){
            var list=Pick(context,region);if(list==null)return;var next=list;
            if(!force&&activeEntry!=null&&activeEntry.EntryId==next.EntryId&&!next.RestartOnEnter)return;
            if(transition!=null)StopCoroutine(transition);transition=StartCoroutine(Switch(next));
        }
        MusicEntryRuntime Pick(MusicContext c,string region){
            if(!byContext.TryGetValue(c,out var list)||list.Count==0)return null;
            var eligible=list.FindAll(x=>string.IsNullOrEmpty(x.Region)||x.Region==region);if(eligible.Count==0)eligible=list;
            float total=0;foreach(var e in eligible)total+=Mathf.Max(.01f,e.Weight);
            float r=Random.value*total;foreach(var e in eligible){r-=Mathf.Max(.01f,e.Weight);if(r<=0)return e;}return eligible[0];
        }
        IEnumerator Switch(MusicEntryRuntime next){
            var old=active;var incoming=old==sourceA?sourceB:sourceA;incoming.clip=next.Clip;incoming.loop=next.Loop;incoming.volume=0;incoming.Play();
            float d=Mathf.Max(0,next.TransitionSeconds);float t=0;
            while(t<d){t+=Time.unscaledDeltaTime;float k=d<=0?1:t/d;old.volume=(1-k)*masterMusicVolume;incoming.volume=k*next.Volume*masterMusicVolume;yield return null;}
            old.Stop();old.volume=0;incoming.volume=next.Volume*masterMusicVolume;active=incoming;activeEntry=next;
        }
        public void Stop(float fade=0.5f){if(transition!=null)StopCoroutine(transition);StartCoroutine(FadeOut(fade));}
        IEnumerator FadeOut(float d){float start=active.volume;float t=0;while(t<d){t+=Time.unscaledDeltaTime;active.volume=start*(1-t/d);yield return null;}active.Stop();activeEntry=null;}
        public void SetMusicVolume(float v){masterMusicVolume=Mathf.Clamp01(v);if(active)active.volume*=masterMusicVolume;}
    }
    [System.Serializable] public sealed class MusicEntryRuntime {
        public string EntryId=System.Guid.NewGuid().ToString("N"); public string Name; public AudioClip Clip; public MusicContext Context;
        public string Region;[Range(0,1)]public float Volume=1;public int Priority=100;public bool Loop=true;public bool RestartOnEnter;
        public float Weight=1;public float TransitionSeconds=1.5f;
    }
    public sealed class AudioMixerBridge : MonoBehaviour { public void SetVolume(AudioCategory category,float normalized){} }
}
