using System.Collections.Generic;
namespace SoulboundLegends.Animation
{
    public interface ISLMotionProvider
    {
        string ProviderId { get; }
        List<SLAnimationAction> BuildActions(string description);
        bool CanHandle(SLAnimationActionType type);
    }
    public static class SLMotionProviderRegistry
    {
        static readonly List<ISLMotionProvider> providers = new();
        public static IReadOnlyList<ISLMotionProvider> Providers => providers;
        public static void Register(ISLMotionProvider p){ if(p!=null&&!providers.Contains(p)) providers.Add(p); }
    }
}
