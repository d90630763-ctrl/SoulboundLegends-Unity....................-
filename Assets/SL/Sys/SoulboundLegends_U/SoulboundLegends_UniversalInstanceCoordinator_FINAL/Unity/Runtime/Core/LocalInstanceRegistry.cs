using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundLegends.InstanceCoordinator
{
    public sealed class LocalInstanceRegistry
    {
        private readonly Dictionary<string, InstanceRecord> records = new();
        private readonly object gate = new();
        public int Count { get { lock(gate) return records.Count; } }
        public void Upsert(InstanceRecord record) { if (record == null || string.IsNullOrWhiteSpace(record.InstanceId)) throw new ArgumentException("Invalid instance record."); lock(gate) records[record.InstanceId] = record; }
        public bool TryGet(string id, out InstanceRecord record) { lock(gate) return records.TryGetValue(id, out record); }
        public bool Remove(string id) { lock(gate) return records.Remove(id); }
        public IReadOnlyList<InstanceRecord> Snapshot() { lock(gate) return records.Values.Select(Clone).ToList(); }
        private static InstanceRecord Clone(InstanceRecord x) => new InstanceRecord { InstanceId=x.InstanceId, InstanceType=x.InstanceType, WorldRegion=x.WorldRegion, ServerId=x.ServerId, MaximumCapacity=x.MaximumCapacity, ReservedSlots=x.ReservedSlots, PlayerIds=new List<string>(x.PlayerIds), PartyIds=new List<string>(x.PartyIds), State=x.State, FailureState=x.FailureState, FailureReason=x.FailureReason, GameplayMode=x.GameplayMode, Ruleset=x.Ruleset, MapEnvironment=x.MapEnvironment, CreatedAt=x.CreatedAt, LastActivityAt=x.LastActivityAt, ExpiresAt=x.ExpiresAt, PersistenceRequired=x.PersistenceRequired, PersistentWorldRelationship=x.PersistentWorldRelationship, Version=x.Version };
    }
}
