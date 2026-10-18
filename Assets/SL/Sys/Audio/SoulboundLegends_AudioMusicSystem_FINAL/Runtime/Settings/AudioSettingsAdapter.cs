using UnityEngine;
namespace SoulboundLegends.AudioMusic
{
    public sealed class AudioSettingsAdapter : MonoBehaviour
    {
        [SerializeField] MonoBehaviour settingsProvider;
        const string Prefix="SL.Audio.";
        public float GetFloat(string key,float fallback=1){if(settingsProvider is ISoulboundSettingsProvider p&&p.TryGetFloat(Prefix+key,out var v))return v;return PlayerPrefs.GetFloat(Prefix+key,fallback);}
        public bool GetBool(string key,bool fallback=true){if(settingsProvider is ISoulboundSettingsProvider p&&p.TryGetBool(Prefix+key,out var v))return v;return PlayerPrefs.GetInt(Prefix+key,fallback?1:0)!=0;}
        public void SetFloat(string key,float value){if(settingsProvider is ISoulboundSettingsProvider p)p.SetFloat(Prefix+key,value);else{PlayerPrefs.SetFloat(Prefix+key,value);PlayerPrefs.Save();}}
        public void SetBool(string key,bool value){if(settingsProvider is ISoulboundSettingsProvider p)p.SetBool(Prefix+key,value);else{PlayerPrefs.SetInt(Prefix+key,value?1:0);PlayerPrefs.Save();}}
    }
}
