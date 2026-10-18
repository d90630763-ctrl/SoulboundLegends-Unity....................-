# Server Event Catalog

Event names emitted by the Rift domain:

- RiftSpawned
- RiftDiscovered
- RiftInstanceCreated
- RiftPlayerEntered
- RiftAttemptConsumed
- RiftPlayerLeft
- RiftBossSpawned
- RiftBossDefeated (record through combat integration / downstream analytics)
- RiftCleared
- RiftTimeExpired
- RiftAttemptsExhausted
- RiftMerged
- CityEngulfed
- CityDestroyed
- RiftRewardsGranted
- RiftReconnect
- RiftInstanceDestroyed
- RiftPersistenceFailure

All events contain Rift ID, city/kingdom context where available, server timestamp and payload.
