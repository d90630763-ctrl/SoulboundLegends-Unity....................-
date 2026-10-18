# Production Checklist

## Before release
- [ ] Play Console app exists.
- [ ] Android application ID matches the server configuration.
- [ ] Google Payments/merchant profile is configured.
- [ ] Products exist in Play Console.
- [ ] Product IDs exactly match the server catalog.
- [ ] Billing integration is included in the Android build.
- [ ] Internal/closed testing track is configured.
- [ ] License testers are configured.
- [ ] Oracle HTTPS endpoint is live.
- [ ] Google service-account credentials are stored only on the server.
- [ ] Google Play Developer API verification is implemented.
- [ ] Master Authority entitlement commit is connected.
- [ ] Purchase token unique constraint is enabled in the real database.
- [ ] Refund/revocation processing is implemented.
- [ ] RTDN validation and reconciliation are implemented.
- [ ] Pending purchase testing passes.
- [ ] Network-loss testing passes.
- [ ] Duplicate callback testing passes.
- [ ] Restore/reconciliation testing passes.

## Never do this
- Never grant currency from Unity alone.
- Never trust the price sent by the client.
- Never put service-account JSON in the APK.
- Never expose database credentials in Unity.
- Never grant a PENDING purchase.
- Never assume a purchase token is valid without server verification.
