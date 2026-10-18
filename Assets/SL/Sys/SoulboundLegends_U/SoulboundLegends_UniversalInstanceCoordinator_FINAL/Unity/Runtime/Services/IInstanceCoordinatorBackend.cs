using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SoulboundLegends.InstanceCoordinator
{
    public interface IInstanceCoordinatorBackend
    {
        Task<InstanceRecord> RequestInstanceAsync(InstanceRequest request, CancellationToken cancellationToken);
        Task<InstanceRecord> GetInstanceAsync(string instanceId, CancellationToken cancellationToken);
        Task<IReadOnlyList<InstanceRecord>> ListInstancesAsync(string type, string region, CancellationToken cancellationToken);
        Task<TransferRecord> RequestTransferAsync(string playerId, string sourceInstanceId, InstanceRequest destinationRequest, CancellationToken cancellationToken);
        Task<bool> ConfirmTransferAsync(string transferId, string playerId, CancellationToken cancellationToken);
        Task<bool> RollbackTransferAsync(string transferId, string reason, CancellationToken cancellationToken);
        Task<bool> ReleasePlayerAsync(string instanceId, string playerId, CancellationToken cancellationToken);
        Task<bool> BeginDrainAsync(string instanceId, CancellationToken cancellationToken);
        Task<bool> DestroyAsync(string instanceId, CancellationToken cancellationToken);
        Task<bool> HeartbeatAsync(string serverId, ServerSnapshot snapshot, CancellationToken cancellationToken);
    }
}
