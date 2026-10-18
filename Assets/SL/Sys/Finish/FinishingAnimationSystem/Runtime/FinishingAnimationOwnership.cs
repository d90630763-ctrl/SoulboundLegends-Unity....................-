using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.Finishing
{
    public class FinishingAnimationOwnership : MonoBehaviour
    {
        private readonly HashSet<string> ownedFinishers = new HashSet<string>();

        public bool Owns(FinishingAnimationDefinition definition)
        {
            if (definition == null) return false;
            if (!definition.requiresOwnership) return true;
            return !string.IsNullOrWhiteSpace(definition.finisherId) &&
                   ownedFinishers.Contains(definition.finisherId);
        }

        // Replace/bridge this with the authoritative inventory/player profile system.
        public void GrantOwnedFinisher(string finisherId)
        {
            if (!string.IsNullOrWhiteSpace(finisherId))
                ownedFinishers.Add(finisherId);
        }

        public bool TryUse(string finisherId)
        {
            return !string.IsNullOrWhiteSpace(finisherId) && ownedFinishers.Contains(finisherId);
        }
    }
}
