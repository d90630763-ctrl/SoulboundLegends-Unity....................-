# SL — In-Game Commerce & Auction Authority System — Final

This system handles **in-game transactions only**. It is separate from the Google Play Commerce Bridge.

## Responsibilities
- Player-to-player auctions
- Auction listings, bids, timers, winners, losing-bid refunds
- Normal city/town/kingdom shops
- General merchant transactions
- Blacksmith shop transactions
- Alchemy shop transactions
- Future shop types through generic merchant IDs
- Server authorization and validation
- Inventory ownership checks
- Currency checks and atomic transfers
- Idempotent transaction requests
- Server-authoritative prices and rewards
- Audit records

## Does NOT handle
- Google Play / Apple / PC real-money purchases
- Bank/card information
- Payment processor verification
- Real-money auctions
- Client-authoritative currency or inventory

## Architecture
Client UI → In-Game Commerce Client → Oracle API → Server Authorization → Transaction/Auction Service → Master Authority → Database

The client only requests actions. The server decides whether the action is legal and commits the result.
