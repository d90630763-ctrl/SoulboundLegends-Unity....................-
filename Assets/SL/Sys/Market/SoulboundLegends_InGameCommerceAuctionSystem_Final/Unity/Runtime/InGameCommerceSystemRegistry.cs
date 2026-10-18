using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.InGameCommerce
{
    public sealed class InGameCommerceSystemRegistry : MonoBehaviour
    {
        [SerializeField] private InGameCommerceClient client;
        [SerializeField] private List<MerchantDefinition> connectedMerchants = new();
        public InGameCommerceClient Client => client;
        public IReadOnlyList<MerchantDefinition> ConnectedMerchants => connectedMerchants;
        public void AddMerchant(MerchantDefinition merchant) { if (merchant != null) connectedMerchants.Add(merchant); }
    }
}
