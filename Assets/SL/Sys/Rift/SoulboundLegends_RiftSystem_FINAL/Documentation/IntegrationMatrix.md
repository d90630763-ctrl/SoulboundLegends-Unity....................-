# Rift Integration Matrix

| SL capability | Rift contract | Required for production |
|---|---|---|
| Adventure World | IRiftAdventureWorldProvider | Yes |
| World Events | IRiftWorldEventsProvider | Recommended |
| World Kingdom | IRiftKingdomProvider | Yes |
| Quest/Guild | IRiftQuestProvider | Yes |
| Monster Generation | IRiftMonsterProvider | Yes |
| General Monster | Same provider/underlying system | Yes |
| Combat | IRiftCombatProvider | Yes |
| General Skill | Existing combat/skill implementation | Yes |
| Skill VFX | IRiftSkillVfxProvider | Recommended |
| Character | IRiftCharacterProvider | Yes |
| Dynamic Instance | IRiftInstanceProvider | Yes |
| Destruction | IRiftDestructionProvider | Yes |
| Portal | IRiftPortalProvider | Yes for boundary enforcement |
| Navigation | IRiftNavigationProvider | Yes |
| NPC | IRiftNpcProvider | Yes |
| Inventory/Equipment | IRiftRewardProvider or reward provider downstream | Yes |
| Rewards | IRiftRewardProvider | Yes |
| Progression | IRiftProgressionProvider | Yes for punishment |
| Persistence | IRiftPersistenceProvider | Yes |
| Database | IRiftDatabaseProvider | Production implementation |
| Server Authority | IRiftServerAuthorityProvider | Yes |
| Server Authorization | IRiftServerAuthorizationProvider | Yes |
| Anti-Cheat | IRiftAntiCheatProvider | Yes |
| Analytics | IRiftAnalyticsProvider | Recommended |
| Lore/Discovery | IRiftLoreProvider | Recommended |
| Notifications | IRiftNotificationProvider | Recommended |
| Audio | IRiftAudioProvider | Recommended |
| UI | IRiftUiProvider | Yes for client presentation |
| Live Ops | IRiftLiveOpsProvider | Recommended |

The Rift System never creates a replacement implementation for an existing SL gameplay system.


## Boundary calls

- Movement/teleport server: call `RiftSystem.ValidateBoundaryEntry` and `ValidateExitBoundary`.
- Portal server: call `RiftSystem.ValidatePortalUse` before accepting a portal destination while a city is engulfed.
