using System;

namespace SoulboundLegends.RiftSystem
{
    public sealed class RiftAuthorityGuard
    {
        private readonly RiftSystemConnectionHub hub;
        private readonly RiftConfiguration config;

        public RiftAuthorityGuard(RiftSystemConnectionHub hub, RiftConfiguration config)
        {
            this.hub = hub;
            this.config = config;
        }

        public bool RequireServer(string action, string playerId, string riftId, string cityId, string nonce, out RiftRequestResult result)
        {
            result = RiftRequestResult.Unauthorized;

            if (!hub.TryGet(RiftCapability.ServerAuthority, out IRiftServerAuthorityProvider authority) ||
                !authority.IsAuthoritativeServer)
                return false;

            if (!authority.ConsumeRateLimit(playerId, action, config.RequestRateLimitCost))
            {
                result = RiftRequestResult.RateLimited;
                return false;
            }

            if (!authority.ValidateRequestNonce(playerId, nonce))
            {
                result = RiftRequestResult.InvalidNonce;
                Report(playerId, riftId, "InvalidNonce", action, 4);
                return false;
            }

            if (hub.TryGet(RiftCapability.ServerAuthorization, out IRiftServerAuthorizationProvider authorization))
            {
                var request = new RiftAuthorizationRequest
                {
                    PlayerId = playerId,
                    Action = action,
                    RiftId = riftId,
                    CityId = cityId,
                    Nonce = nonce,
                    ServerTimestamp = authority.ServerUnixMilliseconds
                };

                if (!authorization.Authorize(request, out _))
                {
                    result = RiftRequestResult.Unauthorized;
                    return false;
                }
            }

            result = RiftRequestResult.Accepted;
            return true;
        }

        public long ServerNow()
        {
            if (hub.TryGet(RiftCapability.ServerAuthority, out IRiftServerAuthorityProvider authority))
                return authority.ServerUnixMilliseconds;
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public void Report(string playerId, string riftId, string anomaly, string details, int severity)
        {
            if (hub.TryGet(RiftCapability.AntiCheat, out IRiftAntiCheatProvider antiCheat))
            {
                antiCheat.Report(new RiftAnomalyReport
                {
                    PlayerId = playerId,
                    RiftId = riftId,
                    AnomalyType = anomaly,
                    Details = details,
                    Timestamp = ServerNow(),
                    Severity = severity
                });
            }
        }
    }
}
