# SL Audio & Music System — FINAL

Standalone Unity package. Runtime code has no dependency on future SL systems. Integration is interface-based.

## External dependencies
- Unity 6 / Unity 6000.x
- UnityEngine.AudioSource
- No third-party package is required.
- Voice System audio generation is optional and requires a connected implementation of `ISoulboundVoiceAudioGenerator`.
- Existing Settings/Persistence is optional and can implement `ISoulboundSettingsProvider`.
- Existing world/terrain systems can implement `ISoulboundSurfaceProvider`.
- Existing gameplay/network systems can implement `ISoulboundGameplayAudioEvents` / `ISoulboundAuthoritativeAudioEvents`.

The package deliberately does not fake those external capabilities.

## Runtime modules
- SoulboundAudioSystem: pooled SFX playback and priority handling.
- AudioEventRouter: structured gameplay/server event entry point.
- FootstepEmitter: surface-aware movement audio.
- AudioGenerationService: optional Voice System bridge.
- SoulboundMusicManager: context/region music, weighted selection and crossfade.
- AudioSettingsAdapter: uses existing settings provider when connected, otherwise PlayerPrefs fallback.
- AudioProfile / MusicProfile: ScriptableObject asset definitions.

## Inspector
Select `SoulboundMusicManager` and use **+ Add Music**. Assign an AudioClip, context, region, volume, priority, loop, weight and transition duration. No code is needed for ordinary music setup.

## Integration contract
Future Soulbound systems should expose one or more of the interfaces in `Runtime/Integration/AudioSystemInterfaces.cs`. The audio package does not require any specific concrete Soulbound system type.

## Important implementation boundary
A Voice System cannot be assumed to generate audio. The package only invokes generation when a connected implementation reports `SupportsGeneration=true`; otherwise it returns a controlled failure and normal/manual audio continues.

## Multiplayer
Clients play audio locally. Server-authoritative events are represented by small typed `AuthoritativeAudioEvent` messages; raw audio is never sent through the gameplay server by this package.
