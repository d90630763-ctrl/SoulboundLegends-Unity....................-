using System;
using System.Collections.Generic;

namespace SoulboundLegends.Commerce
{
    [Serializable]
    public sealed class CommerceProduct
    {
        public string productId;
        public string displayName;
        public string entitlementId;
        public bool consumable;
        public bool enabled = true;
    }

    [Serializable]
    public sealed class GooglePurchaseEvidence
    {
        public string requestId;
        public string productId;
        public string purchaseToken;
        public string packageName;
    }

    [Serializable]
    public sealed class CommerceVerifyResult
    {
        public bool accepted;
        public bool pending;
        public bool alreadyProcessed;
        public string transactionId;
        public string message;
    }

    public enum CommerceState
    {
        Uninitialized,
        Ready,
        Purchasing,
        Pending,
        Verifying,
        Granted,
        Failed
    }
}
