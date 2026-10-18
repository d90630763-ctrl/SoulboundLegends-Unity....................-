INSPECTOR SETUP

1. In Unity, create:
   GameObject -> Create Empty
2. Name it:
   SoulboundVoiceSystem
3. Add Component:
   SL -> Voice Transformation System
4. In Inspector:
   - Create UI On Start = ON
   - Show UI = ON
   - Preview On Start = OFF
   - Save Settings Automatically = ON
5. Press Play.

The UI is created automatically.

If you prefer a controlled UI:
- Turn Create UI On Start OFF.
- Call BuildUI() from your own menu button or another manager.
- Use SetUIVisible(true/false) to open and close the voice settings panel.

IN-GAME SLIDER:
0 = feminine/girly profile
0.5 = neutral
1 = masculine/manly profile

The preset buttons also change the related pitch/deepness/brightness settings.

MOBILE:
Android/iOS microphone permission is required.
Always provide a visible explanation before requesting microphone access in your production game.

MULTIPLAYER:
This system is the voice transformation layer. A multiplayer voice provider/transport should consume the transformed microphone stream. Do not treat the local AudioSource preview as a complete network voice transport.
