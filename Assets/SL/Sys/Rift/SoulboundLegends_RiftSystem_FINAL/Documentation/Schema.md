# Rift Persistence Schema

RiftStateSnapshot.SchemaVersion identifies the serialized schema.

Core fields:
- RiftId
- RiftTypeId
- RiftDifficultyId
- KingdomId
- CityId
- State
- FailureReason
- SpawnUnixMilliseconds
- ExpirationUnixMilliseconds
- StateChangedUnixMilliseconds
- AttemptLimit
- AttemptsConsumed
- CurrentInstanceId
- WorldConsequence
- CompletionRecorded
- RewardsRecorded
- DestructionRecorded
- BossDefinitionId
- BossSpawned
- BossDefeated
- CurrentWave
- Participants
- Monsters
- Rewards
- VersionToken

Production database implementations should store the snapshot atomically and enforce VersionToken compare-and-swap semantics.
