using UnityEngine;
namespace SoulboundLegends.Animation
{
    public static class SLAnimationBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] static void Register(){SLMotionProviderRegistry.Register(new SLDescriptionMotionProvider());}
    }
}
