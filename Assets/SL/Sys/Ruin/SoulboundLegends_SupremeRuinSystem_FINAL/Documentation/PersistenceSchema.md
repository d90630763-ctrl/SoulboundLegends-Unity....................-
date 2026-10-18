# Persistence Schema

## SupremeRuinStateSnapshot

- SchemaVersion
- TrialId
- RuinId
- GodId
- KingdomId
- EntranceId
- InstanceId
- RuinState
- TrialState
- ActiveChallengerPlayerId
- StartedUnixMilliseconds
- LastUpdatedUnixMilliseconds
- TrialDeadlineUnixMilliseconds
- RequiredCreatureCount
- DefeatedCreatureCount
- Creatures[5]
- Participants
- InheritanceAwardState
- AwardedInheritanceId
- FailureReason
- FailureDetail
- Version

## InheritanceRecord

- SchemaVersion
- PlayerId
- CharacterId
- GodId
- InheritanceId
- TrialId
- RuinId
- AcquisitionUnixMilliseconds
- Version

Production storage must make inheritance awarding atomic/idempotent and must enforce one inheritance per player at the database/authoritative progression layer.
