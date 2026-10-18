using System.Collections.Generic; using System.Threading; using System.Threading.Tasks; using UnityEngine;
namespace SoulboundLegends.InstanceCoordinator
{
 public abstract class GameplayAdapterBehaviour : MonoBehaviour, ISystemCapabilityProvider, IInstanceGameplayAdapter
 {
  [SerializeField] private string instanceType="generic"; public string InstanceType=>instanceType; public IEnumerable<string> Capabilities=>new[]{"instance.gameplay."+instanceType}; public bool CanHandle(string capability)=>capability=="instance.gameplay."+instanceType; public object GetCapability(string capability)=>CanHandle(capability)?this:null;
  public virtual Task<bool> ValidateRequestAsync(InstanceRequest request,CancellationToken cancellationToken)=>Task.FromResult(true); public virtual Task OnInstanceCreatedAsync(InstanceRecord instance,CancellationToken cancellationToken)=>Task.CompletedTask; public virtual Task OnPlayersAssignedAsync(InstanceRecord instance,CancellationToken cancellationToken)=>Task.CompletedTask; public virtual Task OnInstanceDrainingAsync(InstanceRecord instance,CancellationToken cancellationToken)=>Task.CompletedTask; public virtual Task OnInstanceDestroyedAsync(InstanceRecord instance,CancellationToken cancellationToken)=>Task.CompletedTask;
 }
}
