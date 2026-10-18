using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.AdventureWorld
{
    public class AdventureWorldAuctionHouseSystem : MonoBehaviour
    {
        public List<AuctionRecord> registry = new();
        public bool SubmitAuction(AuctionRecord auction, IAdventureWorldServerAuthorizationProvider server)
        {
            if (auction == null || string.IsNullOrEmpty(auction.auctionId) || auction.quantity <= 0 || auction.priceAether < 0 || server == null) return false;
            if (registry.Exists(x => x.auctionId == auction.auctionId)) return false;
            if (!server.ValidateOwnership(auction.sellerId, auction.materialId)) return false;
            if (!server.Validate("CreateAuction", auction.sellerId, auction.auctionId)) return false;
            auction.status = "Active"; auction.serverValidated = true; registry.Add(auction); return true;
        }
        public bool CompleteSale(string auctionId, string buyerId, IAdventureWorldServerAuthorizationProvider server)
        {
            var a = registry.Find(x => x.auctionId == auctionId);
            if (a == null || a.status != "Active" || server == null) return false;
            if (!server.Validate("CompleteAuction", buyerId, auctionId)) return false;
            a.buyerId = buyerId; a.status = "Sold"; return true;
        }
    }
}
