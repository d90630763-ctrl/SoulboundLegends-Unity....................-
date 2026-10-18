using System;
using System.Collections.Generic;

namespace SoulboundLegends.Commerce
{
    public interface ICommercePlatformAdapter
    {
        bool IsAvailable { get; }
        void Initialize(Action<bool, string> completed);
        void QueryProducts(IReadOnlyList<CommerceProduct> products, Action<bool, string> completed);
        void Purchase(string productId, string obfuscatedAccountId, Action<GooglePurchaseEvidence, string> completed);
        void Restore(Action<bool, string> completed);
    }
}
