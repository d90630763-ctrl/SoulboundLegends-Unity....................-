using System.Threading;
using System.Threading.Tasks;

namespace SoulboundLegends.InstanceCoordinator
{
    public interface IInstanceGameplayAdapter
    {
        string InstanceType { get; }
        Task<bool> ValidateRequestAsync(InstanceRequest request, CancellationToken cancellationToken);
        Task OnInstanceCreatedAsync(InstanceRecord instance, CancellationToken cancellationToken);
        Task OnPlayersAssignedAsync(InstanceRecord instance, CancellationToken cancellationToken);
        Task OnInstanceDrainingAsync(InstanceRecord instance, CancellationToken cancellationToken);
        Task OnInstanceDestroyedAsync(InstanceRecord instance, CancellationToken cancellationToken);
    }
}
