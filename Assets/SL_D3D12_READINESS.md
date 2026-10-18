# Soulbound Legends — D3D12-Ready Graphics Configuration

Target Unity Editor: **6000.5.6f1**

## Configuration

Windows Standalone is configured for:

1. **Direct3D 12 — primary**
2. **Direct3D 11 — fallback**

The project does **not** contain gameplay code that depends on Direct3D 11. Gameplay, networking, combat, animation, UI, world simulation, server authority, and other systems remain graphics-API independent.

## Important

This package is **D3D12-ready**, not a guarantee that every GPU can run Unity's D3D12 Editor. D3D12 availability depends on the Windows device, GPU driver, Unity version, and device capabilities.

The current development laptop with Intel HD Graphics 4400 should continue to use D3D11 for Editor development if Unity's D3D12 Editor crashes. A stronger D3D12-capable PC can be used for D3D12 validation.

## Unity configuration

The included Editor configurator uses Unity's official PlayerSettings API to disable automatic Windows graphics-API selection and set Direct3D12 first with Direct3D11 as fallback. Unity applies the setting when the project is opened.

Manual location if needed:

**Edit → Project Settings → Player → Other Settings → Rendering → Auto Graphics API for Windows OFF**

Then set:

- Direct3D12
- Direct3D11

## What was NOT done

- No fake D3D12 implementation was added.
- No D3D11-only gameplay architecture was introduced.
- No graphics API was hard-coded into gameplay systems.
- No claim is made that this package has been compiled inside Unity Editor 6000.5.6f1 in this environment.

The package has been structurally validated after creation. Final Unity import/compile/runtime testing must still be performed in Unity Editor.
