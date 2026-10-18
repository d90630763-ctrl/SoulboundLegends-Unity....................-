# Architecture

## Transaction rule
CLIENT REQUEST → AUTHENTICATE → VALIDATE → LOCK/TRANSACTION → COMMIT → RESPONSE

## Normal shop
1. Client asks to buy product X from merchant Y.
2. Server looks up merchant Y and product X.
3. Server obtains the authoritative price.
4. Server checks player session, level/permission requirements, currency and inventory capacity.
5. Server atomically removes currency and grants the item.
6. Server records an audit/economy transaction.
7. Client receives the committed result.

## Blacksmith / Alchemy
These are merchants/services using the same transaction layer.
A future crafting system can call the transaction service rather than creating a second currency-transfer system.
The server validates required materials and outputs.

## Auction
- Listing ownership is checked server-side.
- Listing item is escrowed server-side when listed.
- Bids reserve/hold currency server-side.
- A bid cannot exceed the bidder's available spendable balance.
- Outbid funds are released according to server rules.
- Auction expiration is server-time based.
- Winner is calculated server-side.
- Item and winning currency transfer atomically.
- Duplicate requests use idempotency keys.

## Security
Never trust client values for:
- price
- currency amount
- item ownership
- item quantity
- auction winner
- auction timer
- reward
- transaction success
- shop inventory

The client may send identifiers and desired quantities, but the server determines the legal result.
