# Architecture and system boundaries

Authentication & Identity is the identity proof layer.

It does NOT own:
- gameplay authority
- combat decisions
- inventory truth
- world simulation
- character stat progression
- lobby matchmaking
- transport implementation

It owns:
- account identity
- provider identity
- authentication credentials
- sessions
- tokens
- account security state
- identity audit trail

## [+ Add System] integration model

The package exposes generic connection contracts rather than hard-coded dependencies.

Recommended connected systems:
- `[+ Add System] Account & Session`
- `[+ Add System] Server Authority`
- `[+ Add System] Player Database Backend`
- `[+ Add System] Security / Anti-Cheat`
- `[+ Add System] Network Transport`
- `[+ Add System] Persistence`
- `[+ Add System] Lobby`
- `[+ Add System] Character/Human Generation`
- `[+ Add System] Character Progression`
- `[+ Add System] Master Registry`
- `[+ Add System] Analytics / Diagnostics`

The concrete game project decides which adapters implement these contracts.

## Identity hierarchy

Account ID
→ authenticated account

Player ID
→ game-player identity owned by Player Database

Character ID
→ playable character identity

Session ID
→ one authenticated device/session

External Identity ID
→ internal record for provider + provider subject

Provider subject, not provider email, is the stable Google identity key.
