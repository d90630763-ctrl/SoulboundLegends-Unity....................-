# Provider abstraction

`INetworkTransportProvider` is the stable seam used by gameplay/server systems. `UdpReliableTransportProvider` is the included real implementation. A future provider can replace it without changing the message/replication/authority contracts, provided it preserves channel semantics and security guarantees.
