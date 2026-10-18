using System;
namespace GameSystems.NetworkAdapterContracts {
    public interface INetworkMessage { string MessageType { get; } }
    public interface IRpcEndpoint { void Send(string entityId,string method,string payload); }
    public interface IEntitySerializer { string Serialize<T>(T value); T Deserialize<T>(string value); }
    public readonly struct EntityId: IEquatable<EntityId> {
        public readonly ulong Value; public EntityId(ulong value){Value=value;}
        public bool Equals(EntityId other)=>Value==other.Value; public override bool Equals(object o)=>o is EntityId e&&Equals(e);
        public override int GetHashCode()=>Value.GetHashCode();
    }
}