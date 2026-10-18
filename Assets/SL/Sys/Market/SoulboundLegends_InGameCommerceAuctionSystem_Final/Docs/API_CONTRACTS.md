# API Contracts

## POST /api/commerce/shop/buy
Request: merchantId, productId, quantity, idempotencyKey.
Server determines price and grants the authoritative product.

## POST /api/commerce/auction/list
Request: itemInstanceId, startingBid, buyoutPrice, durationSeconds, idempotencyKey.
Server must verify ownership and escrow the item before a production listing is committed.

## POST /api/commerce/auction/bid
Request: auctionId, bidAmount, idempotencyKey.
Server validates auction state and reserves funds.

## POST /api/commerce/auction/finalize
Request: auctionId, idempotencyKey.
Server checks server time and atomically settles item/currency transfer.

## GET /api/commerce/auction/listings
Returns active listings.
