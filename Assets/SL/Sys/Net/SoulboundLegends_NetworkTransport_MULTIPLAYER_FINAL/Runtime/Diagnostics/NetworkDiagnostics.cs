using System;using System.Collections.Concurrent;
namespace SoulboundLegends.NetworkTransport;
public interface INetworkDiagnostics{void OnMetric(string name,double value);void OnSuspiciousTraffic(string reason,string remote);void OnTransportError(Exception ex);}
public sealed class NetworkDiagnostics:INetworkDiagnostics{public event Action<string,double> Metric;public event Action<string,string> SuspiciousTraffic;public event Action<Exception> TransportError;public void OnMetric(string n,double v)=>Metric?.Invoke(n,v);public void OnSuspiciousTraffic(string r,string remote)=>SuspiciousTraffic?.Invoke(r,remote);public void OnTransportError(Exception e)=>TransportError?.Invoke(e);}
