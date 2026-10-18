# SL — Complete Lobby / Game Entry Specification

## A. Core identity

SL is an original premium mobile fantasy multiplayer game. The lobby is a front-end orchestration system, not a replacement for all gameplay systems. All modes connect through Adventure World.

Progression philosophy:
- Adventure World → Learn & Discover
- Clash / CS → Practice & Sharpen
- Battle Royale → Adapt & Survive
- Siege → Coordinate & Execute

## B. Exact player entry flow

OPEN GAME
→ GAME START / INITIALIZATION
→ LOADING SCREEN
→ ONE-TIME STORY INTRODUCTION
→ ACCOUNT LOGIN / REGISTRATION
→ DEDICATED IN-GAME NAME ENTRY
→ CHARACTER CREATION
→ SAVE CHARACTER
→ MAIN 3D LOBBY
→ MODE SELECTION
→ ADVENTURE WORLD
→ STARTING VILLAGE
→ LEVEL PROGRESSION
→ LV10
→ KINGDOM / ADDITIONAL CONTENT
→ OTHER MODES + PREMIUM FEATURES UNLOCK

The in-game name is a separate real screen. It is not part of account login and not hidden inside character creation.

## C. Startup

Initialize game, check saved data, check connection, initialize services, check account, load required assets, determine new/returning player, trigger first-time introduction when appropriate, continue to account/lobby.

Inspector fields:
Startup Controller, Auto Start, Initialization Delay, Required Systems, Connection Check, Save Data Check, Server Check, First-Time Player Check, On Startup Complete, On Startup Failed, On Connection Failed.

## D. Loading

Real designed loading screen with background, artwork, video slot, 3D character slot, character animation slot, tips, progress bar, percentage, icon, effects, transition, minimum display time and maximum wait time. Loading definitions can be added for startup, Adventure World, Clash/CS, Battle Royale, Siege and events.

## E. One-time story introduction

Inspector-editable video, motion/animation, background picture, audio, music, VFX, subtitles, skip/continue, auto play, skip delay and play-once state. Completion is stored locally for the demo and should be mirrored server-side in production if desired.

## F. Account

Google, Apple and Email/Registration UI hooks. Authentication loading/error/retry/logout hooks. Server authentication required flag. Account identifies the player; in-game name identifies the in-game identity.

## G. In-game name

Dedicated panel with input, preview/error text, confirm, reset/cancel behavior, loading/validation state, duplicate/name-taken hook, profanity/blocked-word filtering, reserved names, character filtering, length limits and server validation.

Inspector:
Name Panel, Name Input Field, Name Preview Text, Confirm Button, Cancel Button, Loading Indicator, Error Panel, Success Message, Minimum Name Length, Maximum Name Length, Allowed Characters, Blocked Characters, Reserved Names, Check Name On Input, Check Name On Confirm, Server Validation Required, Name Taken Message, Invalid Name Message, Too Short Message, Too Long Message, Connection Error Message, On Name Accepted, On Name Rejected, On Name Saved.

## H. Character creation

Male/female bodies, shared skeleton, gender, face, head, hair, hair styles, body size and other appearance options. Preview camera, lighting, save, randomize, reset, confirm and server save hook.

## I. 3D lobby environment

The uploaded lobby picture is the visual reference. The system should build/assemble a real 3D environment rather than use the picture as a flat background. Reference composition includes sky, clouds, floating islands, distant kingdoms/castles, waterfalls/water, platforms/architecture, mountains, vegetation, lighting, fog, atmosphere, particles and magical effects.

Inspector categories:
Environment Profile; Reference Image; Environment Generator; Sky [+ Add]; Clouds [+ Add]; Floating Islands [+ Add]; Kingdoms [+ Add]; Castles [+ Add]; Buildings [+ Add]; Waterfalls [+ Add]; Water [+ Add]; Mountains [+ Add]; Vegetation [+ Add]; Platforms [+ Add]; Bridges [+ Add]; Decorations [+ Add]; Particles [+ Add]; Atmosphere; Lighting [+ Add]; Special Effects [+ Add].

Environment profiles can represent Normal, Season 1, Season 2, Anniversary, Halloween, Winter, Story Event and Special Event. The profile system is expandable.

## J. Character presentation

Actual player character stands in the lobby. Character display point, rotation, scale, idle/entrance animation, weapon display, equipment display, pet display, camera, lighting and VFX hooks are exposed. Character/Pet/Equipment systems remain authoritative sources of their assets.

## K. UI design direction

The UI interaction behavior should complement the 3D lobby and may use Free Fire-style modern mobile multiplayer presentation as a reference: strong selected/pressed/locked states, animated panels, large mode cards, notifications, top bar, left navigation, right social area, bottom utility navigation, prominent Start action, settings categories and 3D previews. Do not copy proprietary Free Fire artwork, logos or characters; SL uses its own assets and identity.

The system supplies default UI graphics, panel/button states, loading graphics, notification graphics, transitions and sample presentation assets.

## L. Main lobby elements

Top player identity, level, currencies, account/social/mail/settings buttons, promotional banner area, left navigation, right online/friends panel, inheritance panel, bottom Vault/Pet/Collection/Rank/Emotes navigation, mode card, Start button and entrance-emote control.

## M. Mode selection

Default modes:
- Adventure World — unlocked at start
- Clash / CS — Lv10
- Battle Royale — Lv10
- Siege — Lv10

Each mode can have name, ID, icon, locked icon, background, preview, description, button, panel, scene, game-system ID, loading screen, required level, pass, quest, unlock, server verification, locked message and unlock message.

## N. Lv10 and premium visibility

Adventure World starts the player in the village. Kingdom access follows the previously specified Lv10 + pass condition. Other competitive modes are locked until Lv10 by default. Store/Luck Royale/premium presentation is hidden or blocked before Lv10 by default so the early experience does not create a money-first impression.

## O. Panels and 3D panels

Panels are configurable by [+ Add Panel]. Profile, Character, Vault, Armory, Inventory, Equipment, Skills, Quests, Events, Store, Friends, Mail, Party, Guild, Settings and Emotes are represented. Vault and Armory support 3D preview cameras/display points rather than only empty Canvas panels.

## P. Entrance emotes

Entrance emotes can be used in lobby, game entry and in-game. Each has name, ID, icon, animation, VFX, audio, duration, camera animation, character position, lobby-use, game-entry-use, in-game-use, level/item/event requirement and server validation.

## Q. Settings

Categories include Graphics, Audio, Controls, HUD, Gameplay, Display and Account. Settings are intended to behave like a full mobile game settings screen: quality, frame rate, effects, shadows, display, master/music/effects/voice/UI volume, control sensitivity, button position/size/transparency, HUD layout/visibility/scale/position, camera/targeting/combat preferences, brightness/UI scale and account actions where allowed.

## R. HUD connection

Lobby connects to General HUD Controller, HUD Preset Manager, HUD Layout Manager and Save System. Presets can store element positions, sizes, transparency and visibility.

## S. Profile / social / mail / events / notifications

Profile shows character, name, level, rank, stats, equipment, progress and background. Social connects Friends, Party, Guild and Chat. Mail supports system messages, rewards, notifications and claim/delete hooks. Events support banners, icons, backgrounds, environment profiles, dates, countdowns, level/quest requirements and rewards. Notifications support icon, sound, animation, duration, priority, panel and formatted text.

## T. Store / draws / currencies

Store and premium sections are server-authoritative and gated by level. Draw types include free/ticket, normal, custom and the reward-removal/increasing-cost Fate Vault concept. Draw definitions expose cost, free attempts, tickets/cost type, reward table hook, pity, guarantee, soft pity, maximum attempts, animation, sound, VFX and server validation. Currency definitions are addable through the Inspector.

## U. Matchmaking

Lobby connects to matchmaking. The project requirement is to find near-level players and balance teams using stats, skills, equipment/build and overall battle power. The actual matchmaking service remains an external/connected system.

## V. System Integration Manager

Use [+ Add System] for Character, Inventory, Equipment, Skills, Quest, Pet, Emote, Social, Party, Guild, Events, Matchmaking, Adventure World, Clash/CS, Battle Royale, Siege, Server, Authentication and future systems.

Each integration exposes system name/ID, controller, manager/panel, opening button/panel, required level, required system, server required, initialization, enable/disable and On Open/Close/Ready/Error events.

Dependencies are represented through required-system IDs and compatibility checks. Example: Alchemy → Inventory → Server; Armory → Equipment → Inventory → Server.

## W. Compatibility checker

Checks Lobby, Character, Inventory, Equipment, Server and all configured system references. It can run on start/in editor, show warnings and optionally block the game on critical errors.

## X. Save / authority split

Client preferences: graphics/audio/control/HUD/UI preferences and local intro completion for demo. Server-owned production data: level, inventory, currency, equipment, unlocks, purchases, draw results, name ownership, rewards and progression. The included local bridge exists only to let the demo run without a backend.

## Y. Folder architecture

Core: Lobby Controller, Navigation, Panel Manager, System Integration, Compatibility Checker.
Entry: Startup, Loading, Introduction, Account, InGameName, Character Creation.
3D Lobby: Environment, Character, Pet, Camera, Lighting, Effects.
Modes: Adventure World, Clash/CS, Battle Royale, Siege.
Player: Profile, Vault, Armory, Inventory, Equipment, Skills.
Social: Friends, Party, Guild, Chat, Mail.
Content: Quests, Events, Rewards, Emotes, Draws.
Economy: Currency, Store, Premium.
Settings: Graphics, Audio, Controls, HUD, Display.
Integration: Other Game Systems, Server, Authentication, Matchmaking, Compatibility Checker.


## FINAL READY-MADE UI REQUIREMENT

This version adds the explicit requirement that the lobby UI is delivered ready-made. The developer is not required to manually create the pages, buttons, panels, mode cards, page headers, loading visuals, navigation, settings categories, emote pages, social pages, inventory/equipment/vault pages, event/quest pages, store/draw pages, or default graphics. The package creates these at runtime from its included visual library.

The only intended production inputs left to the developer are the actual external game-system implementations/data, the final one-time story-introduction video, and the real Character/Pet/Equipment meshes/controllers. The included procedural lobby still creates the 3D fantasy environment baseline from the supplied reference vision, and the UI can be replaced later through Inspector configuration.

### Ready-made page set
Store; Luck Royale; Character; Inventory; Skills; Equipment/Armory; Quests; Events; Vault; Pet; Collection; Rank; Friends; Mail; Profile; Party; Guild; Chat; Settings; Emotes; Entrance Emotes; Mode Selection.

### Ready-made visual states
Normal; highlighted; pressed; selected; disabled; locked; notification; loading; panel open/close; navigation back; page header; banner; content cards; list rows; buttons; mode cards.
