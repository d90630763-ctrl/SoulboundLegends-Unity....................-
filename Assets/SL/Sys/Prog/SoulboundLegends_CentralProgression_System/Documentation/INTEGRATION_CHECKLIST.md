# 5-Year-Old Integration Checklist

1. Open your SL Unity project.
2. Import the `Runtime` and `Editor` folders from this package.
3. Let Unity finish compiling.
4. Click `SL` at the top of Unity.
5. Click `Progression`.
6. Click `Create Config`.
7. Save the config.
8. Click `SL > Progression > Create System Manager`.
9. Select `SBL Progression System`.
10. Put the config into the `Config` box.
11. Put this system manager beside your existing Lobby System Manager.
12. Do NOT create another copy of the progression manager in each scene.
13. Add `SBLStatsPanel` to the Lobby Stats window.
14. Add another `SBLStatsPanel` to the Profile Stats window.
15. Add another `SBLStatsPanel` to the in-game Stats window.
16. Connect the Lobby Stats button to `SBLStatsPanel.Toggle`.
17. Connect the Profile Stats button to `SBLStatsPanel.Toggle`.
18. Connect the in-game Stats button to `SBLStatsPanel.Toggle`.
19. Your existing mode buttons remain responsible for opening Adventure World, CS, BR and Siege.
20. Your matchmaking system can read AdventureLevel, PlayerPower and MatchmakingScore from `SBLMatchmakingBridge`.
21. Your equipment/skill systems can send stat totals into `SetEquipmentStats` and `SetBonusStats`.
22. Your server should ultimately authorize XP, level, ranks and final stats.

The package is intentionally modular: it reads and exposes data rather than replacing your Lobby System.
