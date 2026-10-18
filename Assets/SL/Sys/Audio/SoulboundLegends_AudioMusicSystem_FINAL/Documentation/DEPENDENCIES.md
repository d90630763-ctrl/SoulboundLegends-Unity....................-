# Dependencies and integration boundaries

Required:
1. Unity 6000.x.
2. AudioClip assets for manually assigned music/SFX.

Optional:
- Soulbound Voice System implementing ISoulboundVoiceAudioGenerator.
- Soulbound Settings/Persistence implementing ISoulboundSettingsProvider.
- Terrain/World implementing ISoulboundSurfaceProvider.
- Gameplay event systems implementing the supplied event interfaces.
- Authoritative networking system forwarding AuthoritativeAudioEvent.

Not included:
- AI audio generation model/service.
- Third-party networking.
- Third-party mixer middleware.
- Actual copyrighted music or sound libraries.
