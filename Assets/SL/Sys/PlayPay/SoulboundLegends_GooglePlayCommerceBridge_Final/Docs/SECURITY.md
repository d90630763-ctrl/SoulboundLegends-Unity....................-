# Security Checklist

- HTTPS only in production.
- Short-lived player authentication tokens.
- Never accept a client-provided price as authoritative.
- Never accept a client-provided reward quantity as authoritative.
- Never grant from a client callback alone.
- Verify package/application identity server-side.
- Verify product ID server-side.
- Verify purchase token with Google.
- Require PURCHASED before granting.
- Do not grant PENDING purchases.
- Unique constraint on purchase token.
- Idempotency key/request ID.
- Audit every transaction.
- Keep Google credentials server-side only.
- Rate-limit verification requests.
- Log suspicious token reuse.
- Support refunds/revocations.
- Do not store card numbers or bank credentials.
