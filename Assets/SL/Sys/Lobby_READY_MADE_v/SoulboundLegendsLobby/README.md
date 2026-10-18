# SL — Ready-Made Lobby + Game Entry System v2

## Goal
This version keeps the previously specified Game Entry + Lobby architecture and adds a major requirement: **the visual/UI lobby is ready-made out of the box**.

After importing the package, the developer should only need to connect the real SL systems, supply the final one-time intro video, and connect/import the real Character/Pet/Equipment meshes and controllers. The package already supplies the default pages, navigation, button states, page graphics, banners, panels, loading visuals, notifications, settings page, mode page, emote pages, and the procedural 3D fantasy lobby baseline.

## References
- `Resources/LobbyReference.png` is the supplied lobby reference image.
- The reference is used as the **visual target** for the 3D environment composition; it is not treated as the final 2D lobby background.
- UI behavior and presentation take inspiration from the user's Free Fire-style reference, while the package uses original placeholder/default artwork rather than copying proprietary game assets.

## Import
1. Create/open your Unity project.
2. Unzip this package.
3. Copy `SLLobby/Assets/SLLobby` into your project's `Assets` folder.
4. In Unity, use **Tools → SL → Create Demo Lobby Scene**.
5. Open the generated scene and press Play.
6. Replace the sample intro video with the final story-introduction video when ready.
7. Connect the real Character System/meshes, Pet System, Equipment System and Server/Authentication systems through the Inspector integration fields.

## Ready-made requirement
The package now creates the following pages automatically; they are not blank placeholder canvases:
- Store
- Luck Royale / Draws
- Character
- Inventory
- Skills
- Equipment / Armory
- Quests
- Events
- Vault
- Pet
- Collection
- Rank
- Friends
- Mail
- Profile
- Party
- Guild
- Chat
- Settings
- Emotes
- Entrance Emotes
- Mode Selection

Each page has a designed header, subtitle, content cards/rows, buttons, navigation and state presentation. Connected game systems provide live data rather than forcing the developer to build the page from scratch.

## Entry flow
OPEN GAME → INITIALIZATION → LOADING → ONE-TIME STORY INTRODUCTION → ACCOUNT LOGIN/REGISTRATION → IN-GAME NAME → CHARACTER CREATION → SAVE CHARACTER → MAIN 3D LOBBY → MODE SELECTION → ADVENTURE WORLD.

Adventure World is the starting mode. Clash / CS, Battle Royale and Siege are configured for Lv10. Premium/Store/Draw presentation is hidden/locked before Lv10 according to the existing project requirement.

## What is still intentionally supplied by external game systems
A lobby package cannot invent the player's final multiplayer backend, final character mesh library, final pet library, final equipment database, real authentication provider credentials, or real gameplay scenes. Those are connected systems. The package supplies their UI and integration points so they do not require rebuilding the lobby pages.
