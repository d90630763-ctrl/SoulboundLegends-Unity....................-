# Implementation Guide

## Basic use
1. Import the package into a Unity 6 LTS project.
2. Ensure a player character has an Animator with a valid Humanoid Avatar.
3. Create `SL Animation Definition` from the Assets/Create menu.
4. Write a detailed animation description.
5. Open `SL > Animation > Universal Animation Director`.
6. Generate/Rebuild the action sequence.
7. Select the player and add the runtime animation components.

## Example
Description:
"The warrior draws his sword, performs three slashes, jumps backward and lands."

The compiler produces:
DrawWeapon -> Slash -> Slash -> Slash -> Jump -> Land

## Universal animation
The definition is not tied to a specific player's mesh. Compatible Unity Humanoid characters can use the same definition. Unity Humanoid retargeting adapts the animation to each character.

## IK
The included IK driver supports look-at and hand targets. This is the foundation for weapon/interaction adaptation. Animator IK must be enabled on the relevant Animator layers.

## VFX, audio and gameplay
Every action can carry event IDs. Generated markers expose VFX/audio/gameplay events. A future VFX system can implement `ISLAnimationSystemListener` and register with `SLAnimationSystemBridge`.

## External motion generation
Implement `ISLMotionProvider` for a future AI/local motion provider. This keeps the description compiler, composition, IK, retargeting and VFX architecture independent of a vendor.

## Multiplayer
The animation system must not decide authoritative damage/death. The server-authority system should approve gameplay actions and replicate the sequence/event IDs. Clients then render the animation/VFX.

## Honest limitation
This package is a real Unity procedural animation foundation, not a neural network. It can build action plans and runtime hooks, but it cannot invent arbitrary AAA humanoid choreography from text alone. An advanced motion provider can be added later without replacing this foundation.
