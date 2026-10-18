using System.Collections.Generic;

namespace SoulboundLegends.InstanceCoordinator
{
    public interface ISystemCapabilityProvider
    {
        IEnumerable<string> Capabilities { get; }
        bool CanHandle(string capability);
        object GetCapability(string capability);
    }
}
