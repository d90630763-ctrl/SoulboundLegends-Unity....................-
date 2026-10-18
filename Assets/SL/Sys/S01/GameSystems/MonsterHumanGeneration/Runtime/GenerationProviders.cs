using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace GameSystems.MonsterHumanGeneration {
 public interface IMeshGenerationProvider { Task<Mesh> GenerateMeshAsync(GenerationProfile profile,CancellationToken token); }
 public interface ITextureGenerationProvider { Task<Texture2D> GenerateTextureAsync(GenerationProfile profile,CancellationToken token); }
 public interface IAnimationGenerationProvider { Task<AnimationClip[]> GenerateAnimationsAsync(GenerationProfile profile,GameObject root,CancellationToken token); }
}