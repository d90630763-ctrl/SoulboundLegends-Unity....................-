using System; using System.Numerics;
namespace SoulboundLegends.NetworkTransport;
public enum ConnectionState{Disconnected,Connecting,Authenticating,Connected,Reconnecting,Closing}
public enum TransportChannel:byte{ReliableOrdered=0,ReliableUnordered=1,UnreliableSequenced=2,Unreliable=3}
[Flags] public enum PacketFlags:byte{None=0,Handshake=1,Authenticated=2,AckOnly=4,Compressed=8}
public enum NetworkMessageType:ushort{Heartbeat=1,Ack=2,ClientHello=10,ServerChallenge=11,ClientAuthenticate=12,ServerAccept=13,Disconnect=14,Reconnect=15,ReconnectAccepted=16,Request=100,Response=101,Snapshot=200,Spawn=201,Despawn=202,Event=203}
public readonly struct NetworkEndpoint{public readonly string Host; public readonly int Port; public NetworkEndpoint(string host,int port){Host=host;Port=port;} public override string ToString()=>Host+":"+Port;}
public readonly struct TransformState{public readonly Vector3 Position; public readonly Quaternion Rotation; public readonly Vector3 Velocity; public TransformState(Vector3 p,Quaternion r,Vector3 v){Position=p;Rotation=r;Velocity=v;}}
public readonly struct NetworkStats{public readonly long SentBytes,ReceivedBytes,SentPackets,ReceivedPackets; public readonly float RttMs,PacketLoss,SendKbps,ReceiveKbps; public NetworkStats(long sb,long rb,long sp,long rp,float rtt,float loss,float sk,float rk){SentBytes=sb;ReceivedBytes=rb;SentPackets=sp;ReceivedPackets=rp;RttMs=rtt;PacketLoss=loss;SendKbps=sk;ReceiveKbps=rk;}}
