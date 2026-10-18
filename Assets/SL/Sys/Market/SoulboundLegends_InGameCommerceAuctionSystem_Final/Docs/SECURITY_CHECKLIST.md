# Security Checklist

- HTTPS only
- Short-lived authenticated player tokens
- Server-generated request IDs
- Client-provided idempotency key required for mutations
- Server-authoritative prices
- Server-authoritative auction clock
- Inventory ownership checked on every listing
- Auction item escrow prevents double-selling
- Bid funds reserved server-side
- Atomic database transactions for final settlement
- Unique constraints prevent duplicate purchase/listing settlement
- Rate limits for bidding and transaction endpoints
- Audit log for every currency/item mutation
- Never trust client-reported balances
- Never accept a client-reported auction winner
- Never put server secrets in Unity
- Never store card/bank information in this system
