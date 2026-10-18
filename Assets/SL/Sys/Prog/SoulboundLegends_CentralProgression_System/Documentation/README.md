# SL — Central Progression & Evaluation System

This package is designed to plug into the SL Lobby System without requiring the Lobby to know the internal implementation.

## What it owns

1. Adventure World level: starts 1–100 and can expand in +100 steps up to 1000.
2. Guaranteed per-level stat growth: HP, MP, Attack, Defense, Magic Attack, Magic Defense, Attack Speed, Movement Speed, Critical, Stamina and Skill Power.
3. Separate competitive ranks for CS, BR and Siege.
4. Dynamic class evaluation from actual total stats/equipment/bonuses.
5. Total player power.
6. A matchmaking score that uses Adventure Level and Power.
7. One shared data source for Lobby, Profile and in-game stat displays.

## Important separation

CS/BR/Siege rank does NOT increase or decrease Adventure Level.

Adventure Level and total power can be supplied to matchmaking.

The same progression data can be displayed in:
- Lobby Stats
- Profile Stats
- In-game Stats button/panel

## Integration

1. Import this folder into the Unity project that contains the Lobby System.
2. In the Unity menu choose:
   SL > Progression > Create Config
3. Choose:
   SL > Progression > Create System Manager
4. Select the generated SBL Progression System.
5. Drag the SBL_ProgressionConfig asset into its Config field.
6. Add SBLStatsPanel to a Lobby stats panel, Profile stats panel, and in-game stats panel.
7. Connect each Lobby/Profile/Stats button OnClick to SBLStatsPanel.Toggle or SBLStatsButtonBridge.ToggleStats.
8. Keep the central service on a persistent System Manager object. It uses DontDestroyOnLoad.
9. Your existing Lobby, Character, Equipment, Skill, Matchmaking and Server systems can feed this system through its public APIs.

## No hard dependency on the Lobby

The package intentionally does NOT contain or replace your Lobby scripts. This prevents duplicate Lobby managers, circular references, and broken connections.

SBLLobbyIntegration and SBLMatchmakingBridge are small adapters. Your Lobby can call them or directly read SBLProgressionService.Instance.

## Recommended final architecture

Lobby
 -> SBL Progression System
 -> Adventure World / CS / BR / Siege

All four experiences read the same player progression profile.

Adventure World = character/world progression.
CS = competitive rank.
BR = competitive rank.
Siege = competitive rank.

## Class evaluation

Players are not locked to one permanent class. They can use any supported weapon/job. The evaluator estimates the player's current strongest build, e.g. Warrior, Guardian, Mage, Ranger, Assassin, Support or Spellblade. The evaluator can be expanded later with data-driven weapon/job definitions.

## Server-authoritative production note

For a real online release, the server should be the final authority for XP, Adventure Level, stats, rank/MMR, power and class-evaluation inputs. This package supplies the Unity-side integration layer; it does not pretend to be your backend server.
