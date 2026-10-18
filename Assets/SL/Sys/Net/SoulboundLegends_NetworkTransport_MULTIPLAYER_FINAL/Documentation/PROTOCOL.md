# Protocol

Header is little-endian and fixed-size: Magic(2), Version(1), Flags(1), Channel(1), MessageType(2), ConnectionId(8), Sequence(4), Ack(4), AckBits(4), PayloadLength(2), Nonce(8). Authenticated packets append HMAC-SHA256(32). Maximum packet is configurable and defaults to 1200 bytes to avoid IP fragmentation.

ACK uses the latest received sequence plus a 32-bit history bitmask. Reliable messages remain in a send window until acknowledged. Ordered channels buffer later sequence numbers until the expected sequence arrives. Unreliable-sequenced drops older snapshots.

Handshake:
ClientHello -> ServerChallenge -> ClientAuthenticate -> ServerAccept.
The challenge contains a server nonce. ClientAuthenticate includes the client nonce, server nonce, reconnect token if present, and HMAC(secret, clientNonce||serverNonce||connectionId||authToken). The server validates before accepting application traffic.
