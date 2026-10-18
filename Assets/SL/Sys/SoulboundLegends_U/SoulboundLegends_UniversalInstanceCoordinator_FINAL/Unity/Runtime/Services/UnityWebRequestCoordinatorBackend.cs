using System; using System.Collections.Generic; using System.Threading; using System.Threading.Tasks; using UnityEngine; using UnityEngine.Networking;
namespace SoulboundLegends.InstanceCoordinator
{
 public sealed class UnityWebRequestCoordinatorBackend : IInstanceCoordinatorBackend
 {
  private readonly string baseUrl; private readonly string actor; public UnityWebRequestCoordinatorBackend(string baseUrl,string actor){this.baseUrl=baseUrl.TrimEnd('/');this.actor=actor;}
  public Task<InstanceRecord> RequestInstanceAsync(InstanceRequest r,CancellationToken ct)=>Send<InstanceRecord>(UnityWebRequest.kHttpVerbPOST,$"{baseUrl}/v1/instances",JsonUtility.ToJson(r),ct);
  public Task<InstanceRecord> GetInstanceAsync(string id,CancellationToken ct)=>Send<InstanceRecord>(UnityWebRequest.kHttpVerbGET,$"{baseUrl}/v1/instances/{UnityWebRequest.EscapeURL(id)}",null,ct);
  public async Task<IReadOnlyList<InstanceRecord>> ListInstancesAsync(string type,string region,CancellationToken ct){var list=await SendArray($"{baseUrl}/v1/instances?type={UnityWebRequest.EscapeURL(type)}&region={UnityWebRequest.EscapeURL(region)}",ct);return list;}
  public Task<TransferRecord> RequestTransferAsync(string player,string source,InstanceRequest destination,CancellationToken ct)=>Send<TransferRecord>(UnityWebRequest.kHttpVerbPOST,$"{baseUrl}/v1/transfers",JsonUtility.ToJson(new TransferEnvelope{PlayerId=player,SourceInstanceId=source,Destination=destination}),ct);
  public Task<bool> ConfirmTransferAsync(string id,string player,CancellationToken ct)=>SendBool($"{baseUrl}/v1/transfers/{UnityWebRequest.EscapeURL(id)}/confirm",ct);
  public Task<bool> RollbackTransferAsync(string id,string reason,CancellationToken ct)=>SendBool($"{baseUrl}/v1/transfers/{UnityWebRequest.EscapeURL(id)}/rollback",ct);
  public Task<bool> ReleasePlayerAsync(string instanceId,string player,CancellationToken ct)=>SendBool($"{baseUrl}/v1/instances/{UnityWebRequest.EscapeURL(instanceId)}/release/{UnityWebRequest.EscapeURL(player)}",ct);
  public Task<bool> BeginDrainAsync(string id,CancellationToken ct)=>SendBool($"{baseUrl}/v1/instances/{UnityWebRequest.EscapeURL(id)}/drain",ct);
  public Task<bool> DestroyAsync(string id,CancellationToken ct)=>SendBool($"{baseUrl}/v1/instances/{UnityWebRequest.EscapeURL(id)}",ct, true);
  public Task<bool> HeartbeatAsync(string serverId,ServerSnapshot snapshot,CancellationToken ct)=>SendBool($"{baseUrl}/v1/servers/heartbeat",ct,false,JsonUtility.ToJson(snapshot));
  [Serializable] private sealed class TransferEnvelope{public string PlayerId;public string SourceInstanceId;public InstanceRequest Destination;}
  private async Task<T> Send<T>(string method,string url,string body,CancellationToken ct){using var req=new UnityWebRequest(url,method);if(body!=null){req.uploadHandler=new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));req.SetRequestHeader("Content-Type","application/json");}req.downloadHandler=new DownloadHandlerBuffer();req.SetRequestHeader("X-Actor-Id",actor);using(ct.Register(req.Abort)){var op=req.SendWebRequest();while(!op.isDone)await Task.Yield();}if(req.result!=UnityWebRequest.Result.Success)throw new Exception(req.error);var envelope=JsonUtility.FromJson<ApiEnvelope<T>>(req.downloadHandler.text);if(!envelope.Success)throw new Exception(envelope.Error);return envelope.Data;}
  private async Task<IReadOnlyList<InstanceRecord>> SendArray(string url,CancellationToken ct){using var req=UnityWebRequest.Get(url);req.downloadHandler=new DownloadHandlerBuffer();req.SetRequestHeader("X-Actor-Id",actor);using(ct.Register(req.Abort)){var op=req.SendWebRequest();while(!op.isDone)await Task.Yield();}if(req.result!=UnityWebRequest.Result.Success)throw new Exception(req.error);return JsonUtility.FromJson<ArrayEnvelope>(req.downloadHandler.text).Items;}
  [Serializable] private sealed class ArrayEnvelope{public List<InstanceRecord> Items;}
  private async Task<bool> SendBool(string url,CancellationToken ct,bool delete=false,string body=null){using var req=new UnityWebRequest(url,delete?UnityWebRequest.kHttpVerbDELETE:UnityWebRequest.kHttpVerbPOST);if(body!=null){req.uploadHandler=new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));req.SetRequestHeader("Content-Type","application/json");}req.downloadHandler=new DownloadHandlerBuffer();req.SetRequestHeader("X-Actor-Id",actor);using(ct.Register(req.Abort)){var op=req.SendWebRequest();while(!op.isDone)await Task.Yield();}if(req.result!=UnityWebRequest.Result.Success)throw new Exception(req.error);return JsonUtility.FromJson<BoolEnvelope>(req.downloadHandler.text).Value;}
  [Serializable] private sealed class ApiEnvelope<T>{public bool Success;public T Data;public string Error;} [Serializable] private sealed class BoolEnvelope{public bool Value;}
 }
}
