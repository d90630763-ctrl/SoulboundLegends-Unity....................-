# Integration map and beginner test order

## Scenes included
1. `00_Opening` — optional opening VideoClip slot; generated opening scene.
2. `01_CharacterCreation` — generated character/platform scene.
3. `02_Lobby` — mode-selection buttons.
4. `03_AdventureWorld` — generated village/trees/NPCs/beasts.
5. `04_Clash` — CS-style arena test scene.
6. `05_BattleRoyale` — larger arena test scene.
7. `06_Siege` — fort/wall/gate test scene.
8. `07_SupremeRuin` — five generated divine-boss test actors.
9. `08_Rift` — rift area, beasts and a boss test actor.

## Core runtime test controls
- Move: keyboard W/A/S/D or arrow keys (legacy Unity Input Manager).
- Attack: press `J` to damage the nearest living procedural combatant within range.
- Camera: third-person follow camera is created by the runtime scene controller.
- Lobby: use the on-screen mode buttons to load Adventure, CS/Clash, BR or Siege.
- Intro: select the `SL Runtime` object in `00_Opening`, then assign a VideoClip to `Intro Video` if you have one. Leave it empty if you do not.

## Beginner verification sequence
1. Extract the ZIP to a short path, e.g. `C:\SL\`.
2. In Unity Hub, open the folder `SL\Game` using Unity `6000.5.6f1`.
3. Let the first import finish. Do not press Play while Unity is still importing.
4. Open `Assets/Scenes/00_Opening.unity` and wait for the Console to settle.
5. If the Console has red errors, copy the first error exactly before changing packages.
6. Press Play. Runtime geometry is generated after Play, so the Hierarchy is expected to look sparse before Play.
7. Test movement, the J attack key, and the scene navigation buttons.
8. Test the remaining eight scenes one by one.
9. Only after all local scenes pass should you configure Android build support, external assets, online services and device testing.

## Graphics troubleshooting
The included Editor script configures Windows standalone build graphics APIs; it does not force the Unity Editor itself to use Direct3D 11. If Unity Editor crashes with D3D12 / `CreateCommandAllocator` / `AssetImportWorker` errors, this package cannot guarantee a fix. Try launching the Editor with `-force-d3d11`, update the graphics driver where possible, and avoid repeatedly reimporting the same project until the Editor is stable. Keep a backup before changing graphics settings.

## Scope of this integration
The package combines source modules and a local playable scene-generation layer. The server projects and online features are source foundations only until configured and deployed. A real multiplayer release needs a running server, connected client transport, authoritative gameplay/economy checks, load/security tests and real devices.
