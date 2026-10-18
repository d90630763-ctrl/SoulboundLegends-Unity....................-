# Architecture

```text
Lobby / Store UI
      |
      v
GooglePlayCommerceBridge (Unity)
      |
      | purchase token + authenticated player session
      v
Oracle HTTPS Commerce API
      |
      +--> Google Play Developer API verification
      |
      +--> Commerce transaction/idempotency database
      |
      v
Master Authority
      |
      +--> approved entitlement/economy transaction
      v
Player authoritative state
```

## Responsibilities
### Unity bridge
- Connect to Google Play Billing through an adapter.
- Query products.
- Launch purchase flow.
- Detect purchases.
- Forward purchase tokens to the backend.
- Display pending/success/failure state.
- Refresh authoritative state.

### Commerce backend
- Authenticate the request.
- Validate product IDs against the server catalog.
- Verify purchase token with Google Play.
- Check package/application identity.
- Check purchase state.
- Enforce idempotency.
- Prevent a purchase token being granted twice.
- Acknowledge or consume after entitlement processing.
- Record transaction/refund/revocation information.

### Master Authority
- Decide what the product actually grants.
- Commit the entitlement/economy transaction.
- Keep authoritative player state.

### Oracle
- Hosts the secure backend and database.
