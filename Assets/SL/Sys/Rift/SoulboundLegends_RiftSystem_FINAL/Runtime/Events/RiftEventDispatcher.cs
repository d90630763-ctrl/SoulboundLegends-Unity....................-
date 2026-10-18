using System;
using System.Text;
using UnityEngine;

namespace SoulboundLegends.RiftSystem
{
    public sealed class RiftEventDispatcher
    {
        private readonly RiftSystemConnectionHub hub;
        private readonly RiftAuthorityGuard authority;

        public RiftEventDispatcher(RiftSystemConnectionHub hub, RiftAuthorityGuard authority)
        {
            this.hub = hub;
            this.authority = authority;
        }

        public void Emit(RiftServerEvent evt)
        {
            if (hub.TryGet(RiftCapability.WorldEvents, out IRiftWorldEventsProvider worldEvents))
                worldEvents.Publish(evt);

            foreach (var analytics in hub.GetAll<IRiftAnalyticsProvider>(RiftCapability.Analytics))
                analytics.Record(evt);

            Debug.Log($"[Rift] {evt.EventType} rift={evt.RiftId} player={evt.PlayerId} city={evt.CityId}");
        }

        public RiftServerEvent Create(string type, RiftStateSnapshot snapshot, string playerId = null, string failure = null, string payload = null)
        {
            return new RiftServerEvent
            {
                EventId = Guid.NewGuid().ToString("N"),
                RiftId = snapshot.RiftId,
                PlayerId = playerId,
                CityId = snapshot.CityId,
                KingdomId = snapshot.KingdomId,
                EventType = type,
                FailureReason = failure,
                TimestampUnixMilliseconds = authority.ServerNow(),
                PayloadJson = payload ?? "{}"
            };
        }
    }
}
