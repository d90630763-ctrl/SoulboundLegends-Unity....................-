# Test matrix

| Area | Included automated coverage | Integration/load coverage required |
|---|---|---|
| Serialization | header + primitives + bounds | fuzz/property tests |
| Connect/auth | server/client handshake code | real deployment credentials |
| Disconnect/timeout | timeout path in transport | packet-loss emulator |
| Reliable ordering | sequence/ACK/window code | soak test with loss/reordering |
| AOI | scheduler uses AOI contract | System14 integration |
| Instance isolation | scheduler instance gate | multi-instance server test |
| Authority | pipeline rejects unauthenticated/rate-limited requests | real authority/security/anti-cheat |
| Replication | spawn/update/despawn scheduling | 35+ entity mobile profiling |
| Bandwidth | byte/message rate limiters | sustained load test |
| Reconnect | token fields + reconnect state hooks | full session restore service |
