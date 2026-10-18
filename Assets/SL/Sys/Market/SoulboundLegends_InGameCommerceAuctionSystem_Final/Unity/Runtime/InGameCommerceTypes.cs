using System;
using System.Collections.Generic;

namespace SoulboundLegends.InGameCommerce
{
    [Serializable] public class ShopPurchaseRequest { public string merchantId; public string productId; public int quantity; public string idempotencyKey; }
    [Serializable] public class AuctionListRequest { public string itemInstanceId; public long startingBid; public long buyoutPrice; public int durationSeconds; public string idempotencyKey; }
    [Serializable] public class AuctionBidRequest { public string auctionId; public long bidAmount; public string idempotencyKey; }
    [Serializable] public class AuctionFinalizeRequest { public string auctionId; public string idempotencyKey; }
    [Serializable] public class CommerceResponse { public bool success; public string error; public string rawJson; }
    [Serializable] public class MerchantDefinition { public string merchantId; public string displayName; public string category; }
}
