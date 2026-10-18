# Release checklist

- Replace deployment shared secret through secret storage.
- Integrate real authentication and account/session service.
- Integrate Master Authority, System14 AOI, System24 Matchmaking, Dynamic Instance/Universal Instance Coordinator.
- Integrate authoritative movement/combat simulation.
- Add real persistence/session restoration.
- Enable production DDoS protection and edge rate limiting.
- Run soak tests with packet loss, jitter, duplication and reordering.
- Profile Android/iOS CPU, allocations, GC and radio/network usage.
- Tune tick/send/replication rates per device/network class.
- Add observability backend and alerting.
- Perform security review and penetration testing.
