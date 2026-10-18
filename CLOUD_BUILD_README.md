# SOULBOUND LEGENDS — Unity Cloud Build package

Target editor: **Unity 6000.5.6f1**. The Unity project is at this archive's root: `Assets/`, `Packages/`, `ProjectSettings/`. Do not point a cloud build at a nested `SL/Game` folder.

## Package-resolution hardening

- Removed the old `packages-lock.json`, whose entries were incomplete and could disagree with Package Manager's resolver. Unity will resolve and write a fresh lock file from the minimal manifest.
- Removed optional IDE, collaboration, Timeline, and Test Framework dependencies from the manifest because the runtime/editor project does not require them to build the game.
- Kept UGUI and Unity built-in modules used by the scripts.
- Disabled preview-package opt-in by omitting that nonessential setting.

A package-resolution error can still occur if the build worker cannot reach Unity's package registry, the selected Editor version is unavailable, or the repository/build target is configured incorrectly. The supplied log lines alone do not identify which of those occurred.

## Unity Cloud Automation setup

1. Commit/extract this archive so `Assets`, `Packages`, and `ProjectSettings` are directly inside the repository project root.
2. Select Unity Editor **6000.5.6f1** for the build target.
3. Build target: Android. Install/enable Android Build Support, SDK, NDK, and OpenJDK on the Unity build environment.
4. Allow Package Manager to resolve the manifest. If resolution fails, inspect the earlier Package Manager lines in the full log; the later `Unity player export failed` line is usually a downstream summary, not a root-cause explanation.
5. The Editor build preprocessor generates and registers the nine scenes before a player build.
6. Inspect the generated `Assets/Documentation/BUILDER_LAST_REPORT.md` and test the APK on a real Android device.

## Editor builder

Open `Soulbound Legends → Game Builder`. The builder exposes `BUILD GAME`, `VALIDATE GENERATED PROJECT`, and `BUILD ANDROID APK`. It generates nine scenes: opening, character creation, lobby, Adventure World, Clash Squad, Battle Royale, Siege, Supreme Ruin, and Rift.

## Scope and honest verification status

This archive contains the current project source, editor generation pipeline, procedural character/creature meshes, scene/UI generation, local gameplay foundations, and the additional system source files carried by the project. The generator uses parametric meshes and layered generated parts; it is not a substitute for bespoke AAA character/creature assets, a complete authored animation library, or production backend services. Online sign-in, secure economy, matchmaking, anti-cheat and production multiplayer require external configuration and integration tests.

**Build status:** this environment cannot run Unity Editor 6000.5.6f1 or Unity Cloud Automation, so this archive has not been compiled or exported to an APK here. The package manifest/lockfile have been hardened to address a plausible resolver failure, but a guarantee of zero build errors would require the complete cloud log and a successful Unity build.
