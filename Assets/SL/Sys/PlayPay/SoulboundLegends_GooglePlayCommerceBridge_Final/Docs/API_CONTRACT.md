# API Contract

## POST /api/commerce/googleplay/verify
Authenticated player request.

Request:
```json
{
  "requestId": "uuid",
  "productId": "sbl_diamond_100",
  "purchaseToken": "GOOGLE_PURCHASE_TOKEN",
  "packageName": "com.yourcompany.soulboundlegends"
}
```

Server behavior:
1. Authenticate player.
2. Validate package name against server configuration.
3. Validate product ID against server catalog.
4. Look up purchase token in the database.
5. Query Google Play Developer API.
6. Require PURCHASED state.
7. Resolve the product from the server catalog.
8. Create a unique commerce transaction.
9. Call Master Authority entitlement service.
10. Mark the transaction granted only after the authoritative commit succeeds.
11. Acknowledge or consume the purchase as appropriate.
12. Return authoritative result.

## POST /api/commerce/googleplay/restore
Re-checks purchases associated with the authenticated player. Never trusts local client inventory.

## POST /api/commerce/googleplay/rtdn
Webhook endpoint for Google Real-time Developer Notifications. Verify the notification envelope and then query Google for the latest purchase state rather than trusting notification payloads as the entitlement itself.
