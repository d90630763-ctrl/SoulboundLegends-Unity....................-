# System Connections

## Master Authority
Required authority provider. This system should call Master Authority for authenticated identity and authoritative economy/inventory commits when the Master Authority is available.

## Inventory System
Used to verify ownership, escrow auction items, remove sold items, and grant purchased/won items.

## Economy System
Used for gold/diamond/game-currency balances. The transaction service must perform atomic debit/credit operations.

## Equipment System
Auction and shop item grants can create equipment ownership records after the authoritative item transfer.

## Blacksmith System
Blacksmith UI can use merchant/service transactions for buying materials, paying fees, and receiving server-approved outputs.

## Alchemy System
Alchemy UI can use the same transaction layer for ingredients, fees, and server-approved outputs.

## City/Town/Kingdom Infrastructure
Each physical shop can provide a merchant ID. The commerce system does not need a separate codebase for every location.

## Google Play Commerce Bridge
NO direct dependency. Real-money purchases belong to the separate Google Play Commerce Bridge. If a Google purchase grants currency, the verified result is passed to Master Authority, not through this auction/shop system.
