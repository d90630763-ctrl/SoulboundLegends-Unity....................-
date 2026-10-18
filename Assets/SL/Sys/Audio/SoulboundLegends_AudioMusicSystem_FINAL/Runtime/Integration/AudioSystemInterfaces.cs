using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace SoulboundLegends.AudioMusic
{
    public interface ISoulboundAudioCapabilityProvider { bool TryGetAudioProfile(string key, out AudioProfile profile); }
    public interface ISoulboundSurfaceProvider { SurfaceType GetSurface(Vector3 worldPosition); }
    public interface ISoulboundVoiceAudioGenerator
    {
        bool SupportsGeneration { get; }
        Task<GeneratedAudioResult> GenerateAudioAsync(AudioGenerationRequest request, CancellationToken token);
    }
    public interface ISoulboundGameplayAudioEvents { event Action<AudioRequest> AudioRequested; }
    public interface ISoulboundAuthoritativeAudioEvents { event Action<AuthoritativeAudioEvent> AuthoritativeEvent; }
    public interface ISoulboundSettingsProvider { bool TryGetFloat(string key, out float value); bool TryGetBool(string key, out bool value); void SetFloat(string key,float value); void SetBool(string key,bool value); }
    [Serializable] public struct AuthoritativeAudioEvent { public string EventId; public SoundType SoundType; public AudioCategory Category; public Vector3 Position; public int Priority; public float Intensity; }
    [Serializable] public struct AudioGenerationRequest
    {
        public SoundType SoundType; public AudioCategory Category; public string Source; public string Action; public SurfaceType Material;
        public ElementType Element; public float Intensity; public float Duration; public string Environment; public string CreatureType;
        public WeaponType WeaponType; public string SkillType; public string DesiredStyle; public int VariationCount; public bool Loop; public AudioQuality Quality;
    }
    public struct GeneratedAudioResult { public bool Success; public string Error; public AudioClip Clip; public string AssetId; }
}
