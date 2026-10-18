# Production Manifest Fields

- manifestId: globally unique release ID
- gameId: must be SL
- channel: production, staging, test, etc.
- schemaVersion: update manifest schema
- contentVersion: monotonically increasing content version
- minimumClientVersion / maximumClientVersion: client compatibility window
- serverApiVersion: compatible backend API version
- stage: Draft/Internal/Canary/Staged/Live/Paused/RolledBack/Retired
- applyMode: HotApply/RestartRequired/ClientUpdateRequired
- mandatory: whether the release must be accepted
- rollbackAllowed: release policy flag
- maintenanceRequired: release metadata for maintenance orchestration
- rolloutPercent: 0–100 staged deployment percentage
- dependencies: required release versions
- files: payload files with SHA-256 and size
- configJson: data-driven configuration consumed by game systems
