using UnityEngine;

namespace SoulboundLegends.AdventureWorld
{
    public class AdventureWorldServerAuthorizationBridge : MonoBehaviour
    {
        public MonoBehaviour serverProvider;
        public bool ValidateAction(string action, string playerId, string payload)
        {
            return serverProvider is IAdventureWorldServerAuthorizationProvider server && server.Validate(action, playerId, payload);
        }
        public bool ValidateOwnership(string playerId, string itemId)
        {
            return serverProvider is IAdventureWorldServerAuthorizationProvider server && server.ValidateOwnership(playerId, itemId);
        }
    }
}
