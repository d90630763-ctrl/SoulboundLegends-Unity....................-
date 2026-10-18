# System Connection Map

| System | System ID | Capabilities | Master Authority role |
|---|---|---|---|
| Google Play Commerce | commerce.googleplay | real_money_purchase, receipt_verification, entitlement | Gate + audit + idempotency + route to purchase verifier |
| In-Game Commerce & Auction | commerce.ingame_auction | shop_transaction, blacksmith_transaction, alchemy_transaction, auction, economy_transfer, inventory_transfer | Gate + audit + idempotency + route to domain handlers |
| Future system | your unique ID | your capabilities | Same gate; add server policy/handler |
