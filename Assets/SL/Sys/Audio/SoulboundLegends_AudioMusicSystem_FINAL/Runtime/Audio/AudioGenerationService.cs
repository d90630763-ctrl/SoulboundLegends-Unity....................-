using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace SoulboundLegends.AudioMusic
{
    public sealed class AudioGenerationService : MonoBehaviour
    {
        [SerializeField] MonoBehaviour voiceSystem;
        public bool Available => voiceSystem is ISoulboundVoiceAudioGenerator g && g.SupportsGeneration;
        public Task<GeneratedAudioResult> GenerateAsync(AudioGenerationRequest request,CancellationToken token=default){
            if(voiceSystem is ISoulboundVoiceAudioGenerator g && g.SupportsGeneration)return g.GenerateAudioAsync(request,token);
            return Task.FromResult(new GeneratedAudioResult{Success=false,Error="No connected Voice System audio-generation capability."});
        }
    }
}
