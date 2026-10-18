using System;
using System.Collections.Generic;
namespace GameSystems.MultiplayerAOI {
 public sealed class RequestValidator {
   readonly HashSet<string> used=new HashSet<string>();
   public bool Validate(string requestId,string actorId,float distance,float maxDistance,int permission,float cooldownRemaining,out string reason) {
     if(string.IsNullOrWhiteSpace(requestId)){reason="missing_request_id";return false;}
     if(string.IsNullOrWhiteSpace(actorId)){reason="missing_actor";return false;}
     if(used.Contains(requestId)){reason="duplicate_request";return false;}
     if(distance<0||distance>maxDistance){reason="distance_rejected";return false;}
     if(permission<0){reason="permission_rejected";return false;}
     if(cooldownRemaining>0){reason="cooldown";return false;}
     used.Add(requestId);reason="accepted";return true;
   }
 }
}