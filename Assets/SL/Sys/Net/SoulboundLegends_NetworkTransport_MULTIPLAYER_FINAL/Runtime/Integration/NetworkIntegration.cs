using System;using System.Collections.Generic;
namespace SoulboundLegends.NetworkTransport;
public interface INetworkCapability{string CapabilityId{get;}}
public interface IServerAuthority{bool ValidateRequest(ulong connectionId,ushort messageType,ReadOnlySpan<byte> payload,out string reason);void ApplyRequest(ulong connectionId,ushort messageType,ReadOnlySpan<byte> payload);}
public interface IServerAuthorization:INetworkCapability{bool IsAuthenticated(ulong connectionId);bool CanPerform(ulong connectionId,string action);}
public interface IAntiCheat:INetworkCapability{void Inspect(ulong connectionId,ushort messageType,ReadOnlySpan<byte> payload);}
public interface IAuthenticationSystem:INetworkCapability{bool ValidateToken(string token,out string playerId);}
public interface IAnalytics:INetworkCapability{void Record(string name,double value);}
public interface IInstanceCoordinator:INetworkCapability{bool IsConnectionInInstance(ulong connectionId,ulong instanceId);ulong GetInstance(ulong connectionId);}
public interface IMatchmaking:INetworkCapability{void OnTransportConnected(ulong connectionId);void OnTransportDisconnected(ulong connectionId);}
public sealed class NetworkSystemRegistry{readonly List<INetworkCapability> systems=new();public void AddSystem(INetworkCapability s){if(s!=null&&!systems.Contains(s))systems.Add(s);}public T Find<T>() where T:class,INetworkCapability{foreach(var s in systems)if(s is T t)return t;return null;}public IEnumerable<T> FindAll<T>() where T:class,INetworkCapability{foreach(var s in systems)if(s is T t)yield return t;}}
public sealed class ServerAuthorityPipeline{readonly IServerAuthority authority;readonly IServerAuthorization authz;readonly IAntiCheat anti;readonly RateLimiter limiter;public ServerAuthorityPipeline(IServerAuthority a,IServerAuthorization z,IAntiCheat c,int msg=120,int bytes=131072){authority=a;authz=z;anti=c;limiter=new RateLimiter(msg,bytes);}public bool Process(ulong connectionId,ushort type,byte[] payload,out string reason){reason=null;if(!limiter.Allow(payload.Length)){reason="rate_limit";return false;}if(!authz.IsAuthenticated(connectionId)){reason="not_authenticated";return false;}anti?.Inspect(connectionId,type,payload);if(!authority.ValidateRequest(connectionId,type,payload,out reason))return false;authority.ApplyRequest(connectionId,type,payload);return true;}}
