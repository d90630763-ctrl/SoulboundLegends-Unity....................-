using System;
using System.Collections.Generic;
namespace GameSystems.ServerAuthorityCore {
    public sealed class ServerRequestPipeline {
        readonly HashSet<string> nonces=new HashSet<string>();
        public bool Validate(string requestId,string actorId,int permission) {
            if(string.IsNullOrWhiteSpace(requestId)||string.IsNullOrWhiteSpace(actorId)||permission<0) return false;
            return nonces.Add(requestId);
        }
    }
    public interface IAuthoritativeState { string Snapshot(); void ApplyAuthoritative(string snapshot); }
}