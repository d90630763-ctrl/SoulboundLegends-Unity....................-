using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.Authority
{
    public sealed class GooglePlayCommerceAuthorityAdapter : AuthorityManagedSystem
    {
        protected override string DefaultSystemId => "commerce.googleplay";

        private void Reset()
        {
            gameObject.name = "GooglePlayCommerceAuthority";
        }
    }

    public sealed class InGameCommerceAuctionAuthorityAdapter : AuthorityManagedSystem
    {
        protected override string DefaultSystemId => "commerce.ingame_auction";
        private void Reset()
        {
            gameObject.name = "InGameCommerceAuctionAuthority";
        }
    }
}