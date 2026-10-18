# Architecture

Client Unity -> authenticated HTTPS API -> authority context -> service layer -> repository/transaction layer -> provider abstraction -> PostgreSQL.

Persistent state is authoritative on the server. Unity cache is disposable and is invalidated after mutations. Real-time combat, AOI, movement, active monster simulation, matchmaking queues, and other high-frequency transient state belong to the relevant live server/instance systems, not this database.

## Generic system connections
Unity exposes a serialized list of `MonoBehaviour` connections implementing `ISystemCapabilityProvider`. Future systems advertise capabilities instead of requiring database code changes. Backend services use request/authority contracts rather than concrete game-system dependencies.

## Economy correctness
Critical mutations must execute in a database transaction, lock the affected rows, verify ownership/balance, write a ledger/idempotency record, and commit once. The provided schema prevents negative balances and duplicate idempotency keys. Higher-level purchase/trade services should use the same transaction runner rather than bypassing it.

## Supreme Ruins
Only one active challenger is allowed per ruin. The partial unique index prevents multiple active challengers. Divine creature progress is durable. Inheritance ownership is durable and indexed by God. The application service must execute claim operations transactionally and verify all five required creature records before changing the ruin to Claimed/Completed.

## Rift
Durable state is constrained to Active -> Escalated -> Merging -> Engulfed -> Cleared/Failed. City destruction is a durable consequence and is performed transactionally with the failure transition.

## Economy
`EconomyService` locks the balance row, checks expected version, rejects negative results, writes the ledger and audit record in one transaction, and accepts an idempotency key. `InventoryService` locks the item row and verifies ownership before transfer.
