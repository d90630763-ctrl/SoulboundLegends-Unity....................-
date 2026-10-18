using System; using System.Collections.Generic; using System.Threading; using System.Threading.Tasks; using UnityEngine;
namespace SoulboundLegends.InstanceCoordinator
{
 public sealed class UniversalInstanceCoordinator : MonoBehaviour
 {
  [SerializeField] private InstanceCoordinatorConfig configuration; [SerializeField] private SystemConnector systemConnector;
  private IInstanceCoordinatorBackend backend; private LocalInstanceRegistry registry; private CancellationTokenSource lifetime;
  public event EventHandler<InstanceLifecycleEventArgs> InstanceUpdated; public event EventHandler<TransferEventArgs> TransferUpdated;
  public LocalInstanceRegistry Registry=>registry;
  public void Initialize(IInstanceCoordinatorBackend coordinatorBackend){backend=coordinatorBackend??throw new ArgumentNullException(nameof(coordinatorBackend));registry??=new LocalInstanceRegistry();systemConnector?.Refresh();lifetime=new CancellationTokenSource();}
  public async Task<InstanceRecord> RequestAsync(InstanceRequest request,CancellationToken ct=default){EnsureReady();ValidateLocalRequest(request);var result=await backend.RequestInstanceAsync(request,ct);registry.Upsert(result);InstanceUpdated?.Invoke(this,new(result));return result;}
  public async Task<TransferRecord> TransferAsync(string playerId,string sourceInstanceId,InstanceRequest destination,CancellationToken ct=default){EnsureReady();var result=await backend.RequestTransferAsync(playerId,sourceInstanceId,destination,ct);TransferUpdated?.Invoke(this,new(result));return result;}
  public async Task<bool> ConfirmTransferAsync(string transferId,string playerId,CancellationToken ct=default){EnsureReady();var ok=await backend.ConfirmTransferAsync(transferId,playerId,ct);return ok;}
  public async Task<bool> RollbackTransferAsync(string transferId,string reason,CancellationToken ct=default){EnsureReady();return await backend.RollbackTransferAsync(transferId,reason,ct);}
  public async Task<bool> BeginDrainAsync(string instanceId,CancellationToken ct=default){EnsureReady();return await backend.BeginDrainAsync(instanceId,ct);}
  public async Task<bool> DestroyAsync(string instanceId,CancellationToken ct=default){EnsureReady();return await backend.DestroyAsync(instanceId,ct);}
  public async Task<bool> ReleasePlayerAsync(string instanceId,string playerId,CancellationToken ct=default){EnsureReady();return await backend.ReleasePlayerAsync(instanceId,playerId,ct);}
  public async Task<InstanceRecord> GetAsync(string id,CancellationToken ct=default){EnsureReady();var r=await backend.GetInstanceAsync(id,ct);if(r!=null)registry.Upsert(r);return r;}
  private void EnsureReady(){if(backend==null)throw new InvalidOperationException("Initialize the coordinator with an authoritative backend before use.");}
  private void ValidateLocalRequest(InstanceRequest r){if(r==null)throw new ArgumentNullException(nameof(r));if(string.IsNullOrWhiteSpace(r.InstanceType))throw new ArgumentException("InstanceType is required.");if(string.IsNullOrWhiteSpace(r.WorldRegion))throw new ArgumentException("WorldRegion is required.");if(r.MaximumCapacity<1||r.MinimumCapacity<1||r.MinimumCapacity>r.MaximumCapacity)throw new ArgumentException("Invalid capacity.");}
  private void OnDestroy(){lifetime?.Cancel();lifetime?.Dispose();}
 }
}
