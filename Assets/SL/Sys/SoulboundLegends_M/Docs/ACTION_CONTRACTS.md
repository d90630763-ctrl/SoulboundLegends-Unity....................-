# Authority Action Contracts

Every request contains:
- requestId
- systemId
- capability
- action
- clientTick
- payloadJson

The server verifies the player token, registered system, enabled state, capability, action, payload size and idempotency before the domain handler is allowed to commit state.

## Google Play examples
- `commerce.purchase.submit`
- `commerce.purchase.restore`
- `commerce.purchase.reconcile`

## In-game examples
- `shop.purchase`
- `blacksmith.transaction`
- `alchemy.transaction`
- `auction.create`
- `auction.bid`
- `auction.cancel`
- `auction.settle`
- `auction.refund`
