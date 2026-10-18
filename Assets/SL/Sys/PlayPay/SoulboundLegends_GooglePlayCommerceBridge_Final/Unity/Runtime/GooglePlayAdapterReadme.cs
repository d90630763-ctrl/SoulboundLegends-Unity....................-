/*
 * This file is intentionally documentation-only and compiles as a normal C# file.
 *
 * The Google Play Billing dependency should be supplied by the Google Play Billing
 * Unity/native integration used by the final Android build. The rest of this package
 * does not hard-code a BillingClient dependency, so the commerce architecture can
 * compile in the base Unity project before the platform billing plugin is installed.
 *
 * Implement ICommercePlatformAdapter with your selected Google Play Billing Unity
 * plugin. The adapter must:
 *   - initialize BillingClient
 *   - query product details
 *   - launch BillingClient purchase flow
 *   - return productId + purchaseToken
 *   - support pending purchases
 *   - query purchases again on app foreground/startup
 *   - support restore/reconciliation
 *
 * Do NOT grant game currency in this adapter.
 */
namespace SoulboundLegends.Commerce
{
    internal static class GooglePlayAdapterReadme { }
}
