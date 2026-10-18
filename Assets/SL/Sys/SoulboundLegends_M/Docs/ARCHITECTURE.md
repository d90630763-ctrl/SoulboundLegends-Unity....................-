# Final Architecture

CLIENT SYSTEMS → MASTER AUTHORITY GATE → DOMAIN SERVER HANDLER → DATABASE

## Current connections
1. Google Play Commerce Bridge
2. In-Game Commerce & Auction System
3. Any future AuthorityManagedSystem

## Google Play
Google Play handles real-money payment. The Commerce Bridge sends purchase evidence to the secure server. Master Authority checks the registered commerce system and permitted action. The purchase-verification domain handler must verify the purchase with Google before entitlement is granted.

## In-game commerce
City/Town/Kingdom shops, Blacksmith and Alchemy use the In-Game Commerce & Auction System. Master Authority checks that its transaction action is allowed. The domain handler then validates server prices, inventory, currency and outputs.

## Auction
Master Authority gates auction creation, bidding, cancellation, settlement and refunds. The auction domain handler owns the actual auction state and item/currency escrow.

## Future systems
The Registry is intentionally generic. The Inspector has a `Connected Systems` list with a + button. No second general authorization component is needed.

## Critical rule
The client can request. The server decides. A Unity Inspector connection alone never grants authority to the client.
