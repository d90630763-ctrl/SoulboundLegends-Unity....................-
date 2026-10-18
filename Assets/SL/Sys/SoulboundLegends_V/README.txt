SOULBOUND LEGENDS - VOICE TRANSFORMATION SYSTEM
FINAL READY-MADE UNITY SYSTEM

Purpose:
A standalone Unity voice transformation system for SL.
It provides:
- In-game voice settings panel generated automatically at runtime.
- Girly <-> Neutral <-> Manly slider.
- Pitch control.
- Deepness/brightness controls.
- Presets: Feminine, Neutral, Masculine, Child, Elder, Demon, Robot.
- Microphone preview.
- Start/stop preview.
- Save/load settings with PlayerPrefs.
- Inspector configuration.
- Mobile microphone permission handling.
- No prefab creation is required for the basic setup.

IMPORTANT:
Unity's standard AudioSource pitch changes playback speed as well as pitch. This package therefore provides a practical real-time voice-preview foundation using AudioSource pitch plus filter/EQ-style processing. High-quality formant-preserving gender transformation requires a dedicated DSP/voice-processing plugin or native DSP implementation; this package keeps the system dependency-free.

QUICK SETUP:
1. Import/copy the SLVoice folder into Assets.
2. Create an empty GameObject named VoiceTransformationSystem.
3. Add the VoiceTransformationSystem component.
4. Press Play.
5. The system creates its own UI automatically.
6. For microphone preview, allow microphone permission.
7. On mobile, ensure microphone permission is enabled in Player Settings.

INSPECTOR:
Voice Transformation System exposes:
- Create UI On Start
- Show UI
- UI Canvas Name
- Voice Slider
- Pitch Range
- Deepness
- Brightness
- Output Volume
- Microphone Device
- Preview On Start
- Save Settings Automatically
- PlayerPrefs Key
- Preview AudioSource
- Microphone AudioSource
- Target Output AudioSource

IN-GAME:
The player can:
- Move the main slider left for a more feminine sound.
- Move it right for a more masculine sound.
- Use the preset buttons.
- Adjust pitch/deepness/brightness.
- Start microphone preview.
- Stop preview.
- Save settings.

For multiplayer:
This package handles the local voice transformation layer. Feed the processed voice stream into your chosen multiplayer voice transport/DSP pipeline rather than sending the raw microphone stream.

Tested design target:
Unity 2022.3+ / Unity 6 compatible C# APIs where possible.
