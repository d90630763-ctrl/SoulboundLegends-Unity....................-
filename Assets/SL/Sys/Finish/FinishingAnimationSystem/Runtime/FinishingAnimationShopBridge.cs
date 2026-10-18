using UnityEngine;

namespace SoulboundLegends.Finishing
{
    public class FinishingAnimationShopBridge : MonoBehaviour
    {
        [SerializeField] private FinishingAnimationOwnership ownership;

        // Connect this to the authoritative Shop/Purchase system.
        // Purchase validation, currency deduction, and ownership saving must happen server-side.
        public bool OnAuthoritativePurchaseCompleted(FinishingAnimationDefinition definition)
        {
            if (definition == null || !definition.purchasable)
                return false;

            if (string.IsNullOrWhiteSpace(definition.shopProductId))
                return false;

            if (ownership == null)
                return false;

            ownership.GrantOwnedFinisher(definition.finisherId);
            return true;
        }
    }
}
