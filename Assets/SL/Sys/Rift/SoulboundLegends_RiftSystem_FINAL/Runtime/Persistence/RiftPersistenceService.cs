using System;

namespace SoulboundLegends.RiftSystem
{
    public sealed class RiftPersistenceService
    {
        private readonly RiftSystemConnectionHub hub;

        public RiftPersistenceService(RiftSystemConnectionHub hub) => this.hub = hub;

        public bool Save(RiftStateSnapshot snapshot, string expectedVersion, out string newVersion)
        {
            newVersion = null;
            if (!hub.TryGet(RiftCapability.Persistence, out IRiftPersistenceProvider persistence))
                return false;
            return persistence.TrySave(snapshot, expectedVersion, out newVersion);
        }

        public bool Load(string riftId, out RiftStateSnapshot snapshot)
        {
            snapshot = null;
            if (!hub.TryGet(RiftCapability.Persistence, out IRiftPersistenceProvider persistence))
                return false;
            return persistence.TryLoad(riftId, out snapshot);
        }
    }
}
