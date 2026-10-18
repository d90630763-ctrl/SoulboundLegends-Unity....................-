# Security boundary

Transport authentication proves the connection; it does not prove gameplay correctness. Game authority must independently validate every gameplay request. Use secure deployment secret storage, rotate secrets, add account/session authentication, and put DDoS/rate protection in front of public servers.

The transport rejects malformed packets, oversized payloads, invalid HMACs and duplicate/old sequences. Server-side authority must reject impossible movement, damage, item/currency/progression changes, invalid ownership, invalid instance membership and replayed requests.
