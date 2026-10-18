# Soulbound Legends — Rebuilt Full Package changelog

## Included in this rebuild
- Preserved the original Unity 6000.5.6f1 project and its nine enabled scene entries.
- Preserved the included game-system source modules, server-service source, editor tools, documentation and available concept/texture images.
- Added a reusable procedural actor builder for player, villagers, elemental beasts and divine bosses using Unity's built-in meshes.
- Added transform-driven articulated placeholder rigs and simple procedural idle motion. These are not Humanoid Avatar rigs and do not replace authored/skinned production character assets.
- Added basic enemy approach behavior and health/damage components; the player can test a nearest-target attack with the J key.
- Added direct lobby buttons for Adventure World, CS/Clash, Battle Royale and Siege.
- Added an optional opening-video Inspector field on the Opening scene runtime controller. Assign a VideoClip to `Intro Video` to test playback.
- Kept the runtime world-generation approach and lightweight default package dependencies to avoid introducing more import dependencies.

## What this does not mean
- The package has not been opened or compiled in the Unity Editor by this rebuild process. Static file inspection is not a substitute for an Editor compile/test.
- Built-in Unity meshes are procedural starter geometry. They are not AAA art, final monster sculpts, final buildings, or production animation clips.
- Transform-driven placeholder rigs are not Unity Humanoid Avatars, do not include retargetable animation clips, and need replacement or extension for production animation.
- Multiplayer, server-authoritative economy, authentication, cloud database, Google Play billing and matchmaking services require configuration, deployment, credentials and integration tests. Source files in the archive do not mean those services are live.
- Production publishing still requires performance, security, platform, legal, accessibility and gameplay testing.
