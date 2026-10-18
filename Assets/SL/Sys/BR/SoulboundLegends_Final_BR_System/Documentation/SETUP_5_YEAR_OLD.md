# BR Setup — Baby Steps

1. Open your Unity SL project.
2. Make a folder named `SL/BR`.
3. Copy the `Runtime` folder into it.
4. Copy the `Editor` folder into it.
5. Wait for Unity to finish importing.
6. Create an empty GameObject in a setup scene.
7. Name it `BR World Generator`.
8. Add the `BRWorldGenerator` component.
9. Click `AUTO-CREATE ALL CONNECTION SLOTS`.
10. Drag your existing systems into the matching slots.
11. Your Terrain Generator and Infrastructure Generator are especially important.
12. Make sure those generators implement `IBRWorldGenerationProvider`.
13. Click `VALIDATE BR CONNECTIONS`.
14. Fix any missing required systems.
15. Click `GENERATE FINAL BR WORLD`.
16. Unity creates the generated BR world root and BR map data.
17. Generation providers create the actual 3D content they own.
18. Connect the generated BR world to the existing Lobby/Scene Loading flow.
19. Do not create a second BR HUD.
20. Do not create a separate BR weapon/skill/power progression system.
