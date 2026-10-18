using System;
using System.Collections.Generic;

namespace SoulboundLegends.Authority
{
    public enum AuthorityResultCode
    {
        Success, Unauthorized, Forbidden, InvalidRequest, InvalidState,
        RateLimited, Conflict, NotFound, Duplicate, ServerError
    }

    [Serializable]
    public sealed class AuthorityRequest
    {
        public string requestId;
        public string systemId;
        public string action;
        public string capability;
        public long clientTick;
        public Dictionary<string, object> payload;
    }

    [Serializable]
    public sealed class AuthorityResponse
    {
        public AuthorityResultCode code;
        public string requestId;
        public string systemId;
        public string action;
        public string message;
        public string stateVersion;
        public string serverTimeUtc;
        public Dictionary<string, object> result;
    }

    [Serializable]
    public sealed class PlayerSnapshot
    {
        public string playerId;
        public string characterId;
        public long revision;
        public int level;
        public long xp;
        public long hp;
        public long mp;
        public long attack;
        public long defense;
        public long attackSpeed;
        public long currencySoft;
        public long currencyPremium;
        public string rank;
        public string worldId;
        public string instanceId;
        public string[] ownedSkills;
    }

    public interface IAuthoritySystem
    {
        string SystemId { get; }
        IEnumerable<string> Capabilities { get; }
        void OnAuthorityReady(MasterAuthorityClient authority);
        void OnAuthoritySnapshot(PlayerSnapshot snapshot);
    }

    public interface IAuthorityRequestSource
    {
        string SystemId { get; }
        IEnumerable<string> Capabilities { get; }
        void AttachAuthority(MasterAuthorityClient authority);
    }
}