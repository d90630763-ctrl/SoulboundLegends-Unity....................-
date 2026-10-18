# Soulbound Legends — One-Click Builder Guide

## First run
1. Extract the ZIP.
2. Open `SL/Game` in Unity Hub using Unity `6000.5.6f1`.
3. Let Unity finish importing packages and compiling scripts. If the Console has red errors, fix those before proceeding.
4. From the Unity top menu choose **Soulbound Legends → Game Builder**.
5. In **Opening Cinematic**, drag your intro video into **Intro Video (optional)**. The slot is optional during setup.
6. Review generation settings. The defaults create all nine scenes.
7. Click **BUILD GAME** and wait for the progress bar to finish.
8. Read the report shown in the Builder window and the generated file `Assets/Documentation/BUILDER_LAST_REPORT.md`.
9. Open `Assets/Scenes/00_Opening.unity` and press Play. Use Continue, then the guest testing path to reach character creation and the lobby.
10. In the lobby, use the mode buttons to enter Adventure World, CS, BR or Siege. In gameplay scenes, test the quest/inventory/map panels, attack, elemental skill and quest reward buttons.
11. To build an APK, install Android Build Support (SDK, NDK and OpenJDK) through Unity Hub, then click **BUILD ANDROID APK**. The default output is `Builds/Android/SoulboundLegends.apk`.

## What the report means
- **PASS** means the editor-side asset check found the expected scene and build-settings entry.
- **WARNING** means the feature needs an external asset/provider/service or runtime test. Warnings are not proof of failure, but they are not to be ignored.
- **ERROR** means a required generated scene asset is missing and the project should not be treated as build-ready.

## Important
The builder generates base world geometry and procedural actors from Unity primitives, so the project can be assembled and explored while higher-quality authored models, textures, animation clips, VFX, and audio are supplied. The Builder does not turn those primitive meshes into premium art automatically. Google sign-in, email sign-in, production multiplayer, anti-cheat deployment and online databases are not enabled merely by generating scenes. Those systems need provider setup and real tests.
