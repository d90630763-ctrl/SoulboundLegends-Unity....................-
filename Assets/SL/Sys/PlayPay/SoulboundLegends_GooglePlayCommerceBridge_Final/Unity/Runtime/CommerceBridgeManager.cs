using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.Commerce
{
    public sealed class CommerceBridgeManager : MonoBehaviour
    {
        [Header("Server")]
        [SerializeField] private string backendBaseUrl = "https://YOUR-ORACLE-DOMAIN";

        [Header("Google Play")]
        [SerializeField] private string androidPackageName = "com.yourcompany.soulboundlegends";
        [SerializeField] private List<CommerceProduct> products = new List<CommerceProduct>();

        [Header("Debug")]
        [SerializeField] private bool logCommerceEvents = true;

        public CommerceState State { get; private set; } = CommerceState.Uninitialized;
        public IReadOnlyList<CommerceProduct> Products => products;
        public event Action<CommerceState> StateChanged;
        public event Action<CommerceVerifyResult> PurchaseVerified;

        private ICommercePlatformAdapter platformAdapter;
        private CommerceBackendClient backend;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            backend = new CommerceBackendClient(backendBaseUrl, GetAccessToken);
            State = CommerceState.Ready;
            RaiseState();
        }

        public void ConfigurePlatformAdapter(ICommercePlatformAdapter adapter)
        {
            platformAdapter = adapter;
        }

        public void InitializePlatform()
        {
            if (platformAdapter == null)
            {
                Fail("No Google Play platform adapter is installed.");
                return;
            }

            platformAdapter.Initialize((ok, message) =>
            {
                if (!ok) Fail(message);
                else
                {
                    State = CommerceState.Ready;
                    RaiseState();
                    platformAdapter.QueryProducts(products, (queryOk, queryMessage) =>
                    {
                        if (!queryOk) Fail(queryMessage);
                    });
                }
            });
        }

        public void Buy(string productId)
        {
            CommerceProduct product = products.Find(p => p != null && p.enabled && p.productId == productId);
            if (product == null)
            {
                Fail("Unknown or disabled product.");
                return;
            }

            if (platformAdapter == null || !platformAdapter.IsAvailable)
            {
                Fail("Google Play Billing is not available.");
                return;
            }

            State = CommerceState.Purchasing;
            RaiseState();
            platformAdapter.Purchase(productId, GetObfuscatedAccountId(), OnPlatformPurchase);
        }

        public void RestorePurchases()
        {
            if (platformAdapter == null)
            {
                Fail("Google Play platform adapter is not installed.");
                return;
            }

            platformAdapter.Restore((ok, message) =>
            {
                if (!ok) Fail(message);
            });
        }

        private void OnPlatformPurchase(GooglePurchaseEvidence evidence, string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                Fail(error);
                return;
            }

            if (evidence == null || string.IsNullOrEmpty(evidence.purchaseToken))
            {
                Fail("Google Play returned no purchase token.");
                return;
            }

            evidence.packageName = androidPackageName;
            State = CommerceState.Verifying;
            RaiseState();
            StartCoroutine(backend.VerifyGooglePurchase(evidence, OnVerificationComplete));
        }

        private void OnVerificationComplete(CommerceVerifyResult result)
        {
            if (result == null)
            {
                Fail("Empty verification response.");
                return;
            }

            PurchaseVerified?.Invoke(result);

            if (result.pending)
            {
                State = CommerceState.Pending;
                RaiseState();
                return;
            }

            if (result.accepted || result.alreadyProcessed)
            {
                State = CommerceState.Granted;
                RaiseState();
                return;
            }

            Fail(result.message);
        }

        private void Fail(string message)
        {
            if (logCommerceEvents)
                Debug.LogWarning("[SBL Commerce] " + message);
            State = CommerceState.Failed;
            RaiseState();
        }

        private void RaiseState()
        {
            StateChanged?.Invoke(State);
        }

        private string GetAccessToken()
        {
            // Connect this to your existing authenticated player/session system.
            return string.Empty;
        }

        private string GetObfuscatedAccountId()
        {
            // Connect this to your authenticated account ID and hash it server-side.
            return string.Empty;
        }
    }
}
