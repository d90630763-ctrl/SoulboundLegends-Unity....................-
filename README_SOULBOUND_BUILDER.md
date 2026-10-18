# Soulbound Legends — Unity 6 Builder Package

Target Unity Editor: **6000.5.6f1**. Open the **`SL/Game`** folder in Unity Hub.

## One-click editor generation
The custom **Soulbound Legends → Game Builder** window includes an optional intro-video slot, **BUILD GAME**, **VALIDATE GENERATED PROJECT**, and **BUILD ANDROID APK**. Build Game generates nine scenes, scene geometry, UI, procedural actors, navigation and Build Settings entries. It also builds three front-facing character previews in the Character Creation scene.

## Procedural character system upgrade
The shared runtime actor factory now creates stylized characters from parametric meshes: anatomy, face details, hair, layered clothing, armor, boots, accessories and elemental accents. A transform rig supplies idle, walk/run, attack, heavy-attack windup/strike/recovery, block, dodge and jump motions. Optional look and hand targets support lightweight procedural IK alignment. Attack/Heavy Attack/Block/Dodge UI actions call the rig. System 01's profile mesh generator now creates a detailed composite mesh asset and higher-resolution patterned textures; generated prefabs use the same clothed actor factory as the Build Game pipeline.

## Build steps
1. Open `SL/Game` in Unity Hub and wait for compilation.
2. Choose **Soulbound Legends → Game Builder**.
3. Click **BUILD GAME**.
4. Click **VALIDATE GENERATED PROJECT** and read `Assets/Documentation/BUILDER_LAST_REPORT.md`.
5. Install Android Build Support in Unity Hub before attempting **BUILD ANDROID APK**.

## Important limits
The generated actors are an original stylized foundation, not guaranteed Free Fire-equivalent production art. Procedural animation is generated at runtime from motion primitives, not from motion-captured clips. Optional IK targets must be assigned to drive aiming/hand placement. Google/email sign-in, live multiplayer, databases, anti-cheat and billing require real provider/server configuration and tests. The package has not been compiled or run inside Unity in this environment, so verify in Unity 6000.5.6f1 before publishing.
