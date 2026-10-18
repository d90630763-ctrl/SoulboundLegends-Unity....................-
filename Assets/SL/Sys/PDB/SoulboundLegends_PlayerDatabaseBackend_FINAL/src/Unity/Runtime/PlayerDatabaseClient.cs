using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace SoulboundLegends.PlayerDatabase;
[CreateAssetMenu(menuName="Soulbound Legends/Player Database/Client Configuration")]
public sealed class PlayerDatabaseClientConfig : ScriptableObject { public string BaseUrl="https://localhost:7045"; [Range(1,5)] public int MaxRetries=3; public int TimeoutSeconds=15; }
public interface ISystemCapabilityProvider { IEnumerable<string> GetCapabilities(); }
public sealed class PlayerDatabaseSystemConnections : MonoBehaviour { [Tooltip("Generic [+ Add System] connections. Add any future system that exposes capabilities.")] [SerializeField] List<MonoBehaviour> systems=new(); public bool HasCapability(string capability){foreach(var s in systems) if(s is ISystemCapabilityProvider p && System.Linq.Enumerable.Contains(p.GetCapabilities(),capability,StringComparer.OrdinalIgnoreCase)) return true; return false;} public IReadOnlyList<MonoBehaviour> ConnectedSystems=>systems; }
public sealed class PlayerDatabaseClient : IDisposable
{
 readonly HttpClient http; readonly PlayerDatabaseClientConfig config; string? token; readonly Dictionary<Guid,PlayerSnapshotDto> cache=new();
 public PlayerDatabaseClient(PlayerDatabaseClientConfig c){config=c;http=new HttpClient{BaseAddress=new Uri(c.BaseUrl),Timeout=TimeSpan.FromSeconds(c.TimeoutSeconds)};}
 public void SetAuthenticationToken(string jwt){token=jwt;http.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",jwt);}
 public async Task<PlayerSnapshotDto?> LoadPlayerAsync(Guid playerId,CancellationToken ct){var r=await SendAsync($"api/v1/player/{playerId}",HttpMethod.Get,null,ct);if(r.StatusCode==System.Net.HttpStatusCode.NotFound)return null;r.EnsureSuccessStatusCode();var dto=JsonSerializer.Deserialize<PlayerSnapshotDto>(await r.Content.ReadAsStringAsync(ct));if(dto!=null)cache[playerId]=dto;return dto;}
 public async Task<MutationResultDto> MutateAsync(Guid playerId,string operation,long expectedVersion,object payload,string idempotencyKey,CancellationToken ct){var body=JsonSerializer.Serialize(new{operation,idempotencyKey,expectedVersion,payload});var r=await SendAsync($"api/v1/player/{playerId}/mutation",HttpMethod.Post,new StringContent(body,Encoding.UTF8,"application/json"),ct);r.EnsureSuccessStatusCode();var result=JsonSerializer.Deserialize<MutationResultDto>(await r.Content.ReadAsStringAsync(ct))!;cache.Remove(playerId);return result;}
 async Task<HttpResponseMessage> SendAsync(string path,HttpMethod method,HttpContent? content,CancellationToken ct){Exception? last=null;for(int i=0;i<config.MaxRetries;i++){try{using var req=new HttpRequestMessage(method,path){Content=content};var res=await http.SendAsync(req,ct);if((int)res.StatusCode>=500 && i+1<config.MaxRetries){await Task.Delay(100*(i+1)*(i+1),ct);continue;}return res;}catch(HttpRequestException ex)when(i+1<config.MaxRetries){last=ex;await Task.Delay(100*(i+1)*(i+1),ct);}}throw last??new HttpRequestException("Database backend request failed.");}
 public void Invalidate(Guid playerId)=>cache.Remove(playerId); public void ClearCache()=>cache.Clear(); public void Dispose()=>http.Dispose();
}
[Serializable] public sealed class PlayerSnapshotDto {public Guid playerId;public Guid accountId;public Guid characterId;public string characterName="";public string gender="";public int level;public long xp;public string rank="";public long version;}
[Serializable] public sealed class MutationResultDto {public string idempotencyKey="";public long newVersion;public object? result;}
