using UnityEngine;
namespace SoulboundLegends.NetworkTransport;
[CreateAssetMenu(menuName="Soulbound Legends/Networking/Network Transport Config")]
public sealed class NetworkTransportConfig:ScriptableObject {
 public string serverAddress="127.0.0.1"; public int serverPort=27015; public int clientBindPort=0;
 [Min(1)] public int tickRate=30; [Min(1)] public int sendRate=30; [Min(0.5f)] public float heartbeatSeconds=1f; [Min(1f)] public float timeoutSeconds=8f; [Min(0f)] public float reconnectWindowSeconds=30f;
 [Min(256)] public int maxPacketBytes=1200; [Min(1)] public int maxReliableInFlight=256; [Min(1)] public int maxMessagesPerSecond=120; [Min(1)] public int maxBytesPerSecond=128*1024; [Min(1)] public int replicationBudgetBytesPerSecond=64*1024;
 public bool enableHmac=true; [TextArea] public string deploymentSharedSecret=""; public bool logSuspiciousTraffic=true; [Min(1)] public int maxReplicationEntities=35;
 public int reconnectGraceMilliseconds=>Mathf.Max(1000,Mathf.RoundToInt(reconnectWindowSeconds*1000));
}
