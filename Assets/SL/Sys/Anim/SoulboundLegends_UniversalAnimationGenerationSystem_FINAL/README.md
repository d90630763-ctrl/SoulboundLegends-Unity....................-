# SL Universal Animation Generation System — FINAL

Unity 6 LTS foundation for description-driven procedural animation composition.

## Workflow
Description -> action breakdown -> motion primitives -> timing -> IK/root-motion hooks -> composition -> reusable Humanoid sequence -> VFX/Camera/Audio event hooks.

## Important truth
This package is a Unity-side procedural/composition system. It does not contain a neural text-to-motion model. It can create basic procedural motion and compose complex actions from those basic actions. An external motion provider can later be plugged into `ISLMotionProvider` without replacing the architecture.

## Universal character support
Use Unity Humanoid Avatars. A generated animation definition is authored at the Humanoid level and can be retargeted to compatible player characters.

## Install
Extract into the Unity project root, reopen Unity, wait for compilation, then use:
SL > Animation > Universal Animation Director
