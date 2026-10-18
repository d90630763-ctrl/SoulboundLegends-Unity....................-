#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SoulboundLegends.CSGenerator;

namespace SoulboundLegends.CSGenerator.Editor
{
    [CustomEditor(typeof(CSWorldGenerator))]
    public class CSWorldGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox(
                "FINAL CS WORLD GENERATOR\n\n" +
                "1. Connect the required existing systems.\n" +
                "2. Make sure Terrain and Infrastructure generation systems are connected.\n" +
                "3. Press GENERATE FINAL CS WORLD.\n\n" +
                "The generator creates the five parent maps and 35 combat sites, applies CS rules, " +
                "and invokes connected generation-capable systems.",
                MessageType.Info);

            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();

            var generator = (CSWorldGenerator)target;

            EditorGUILayout.Space(12);
            if (GUILayout.Button("AUTO-CREATE ALL CONNECTION SLOTS", GUILayout.Height(32)))
            {
                Undo.RecordObject(generator, "Create CS System Slots");
                CreateSlots(generator);
                EditorUtility.SetDirty(generator);
            }

            if (GUILayout.Button("VALIDATE CONNECTIONS", GUILayout.Height(28)))
            {
                if (CSGenerationValidation.Validate(generator, out var errors))
                    EditorUtility.DisplayDialog("CS Generator", "All required connection slots are assigned.", "OK");
                else
                    EditorUtility.DisplayDialog("Missing CS Connections", string.Join("\n\n", errors), "OK");
            }

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("GENERATE FINAL CS WORLD", GUILayout.Height(44)))
            {
                GUI.backgroundColor = Color.white;
                generator.GenerateFinalCSWorld();
                EditorUtility.SetDirty(generator);
            }
            GUI.backgroundColor = Color.white;
        }

        private void CreateSlots(CSWorldGenerator g)
        {
            g.systems = new System.Collections.Generic.List<CSGeneratorSystemSlot>();
            Add(g, "General Lobby System", CSGeneratorSystemType.Lobby, true, "Lobby entry/exit integration.");
            Add(g, "General HUD System", CSGeneratorSystemType.HUD, true, "Shared HUD presentation.");
            Add(g, "Character System", CSGeneratorSystemType.Character, true, "Persistent player character.");
            Add(g, "FOG Character / Controller System", CSGeneratorSystemType.CharacterController, true, "Movement/controller.");
            Add(g, "Combat System", CSGeneratorSystemType.Combat, true, "Authoritative combat.");
            Add(g, "Skill System", CSGeneratorSystemType.Skills, true, "Adventure World skills.");
            Add(g, "Equipment System", CSGeneratorSystemType.Equipment, true, "Adventure World equipment.");
            Add(g, "Inventory / Item System", CSGeneratorSystemType.Inventory, true, "Owned items.");
            Add(g, "Stats / Battle Power System", CSGeneratorSystemType.StatsBattlePower, true, "Power and matchmaking.");
            Add(g, "Animation System", CSGeneratorSystemType.Animation, false, "Shared animations.");
            Add(g, "Server Authority System", CSGeneratorSystemType.ServerAuthority, true, "Authoritative server validation.");
            Add(g, "Networking / Multiplayer System", CSGeneratorSystemType.Networking, true, "Network synchronization.");
            Add(g, "Account / Player Data System", CSGeneratorSystemType.AccountData, true, "Account/progression.");
            Add(g, "Scene / World Loading System", CSGeneratorSystemType.SceneLoader, true, "World loading.");
            Add(g, "Audio System", CSGeneratorSystemType.Audio, false, "Audio.");
            Add(g, "Camera System", CSGeneratorSystemType.Camera, false, "Gameplay/spectator camera.");
            Add(g, "Party / Friends System", CSGeneratorSystemType.PartyFriends, false, "Party queue.");
            Add(g, "Chat / Communication System", CSGeneratorSystemType.Chat, false, "Team communication.");
            Add(g, "Rank / Season System", CSGeneratorSystemType.RankSeason, false, "CS rank/season.");
            Add(g, "Reward / Progression System", CSGeneratorSystemType.RewardsProgression, true, "Rewards.");
            Add(g, "Game State System", CSGeneratorSystemType.GameState, true, "Global state.");
            Add(g, "Time Synchronization System", CSGeneratorSystemType.TimeSync, true, "Server time.");
            Add(g, "Physics / Collision System", CSGeneratorSystemType.PhysicsCollision, true, "Physics/collision.");
            Add(g, "Localization / Text System", CSGeneratorSystemType.Localization, false, "Localization.");
            Add(g, "Analytics / Match History System", CSGeneratorSystemType.AnalyticsHistory, false, "Analytics/history.");
            Add(g, "Reporting / Punishment System", CSGeneratorSystemType.ReportingModeration, false, "Moderation.");
            Add(g, "Anti-Cheat System", CSGeneratorSystemType.AntiCheat, true, "Competitive security.");

            Add(g, "Terrain Generator System", CSGeneratorSystemType.TerrainGenerator, true, "Builds parent terrain and site terrain.");
            Add(g, "Infrastructure Generator System", CSGeneratorSystemType.InfrastructureGenerator, true, "Builds buildings, structures, cover and routes.");
            Add(g, "Water / Environment System", CSGeneratorSystemType.WaterEnvironment, true, "Ocean/underwater/water environment.");
            Add(g, "Weather / Lighting System", CSGeneratorSystemType.WeatherLighting, false, "Environment atmosphere.");
            Add(g, "Terrain Destruction System", CSGeneratorSystemType.TerrainDestruction, false, "Destructible environment.");
            Add(g, "NPC Generator System", CSGeneratorSystemType.NPCGenerator, false, "Optional world NPCs.");
            Add(g, "Monster Generator System", CSGeneratorSystemType.MonsterGenerator, false, "Optional world creatures.");
            Add(g, "World NPC Assembly System", CSGeneratorSystemType.WorldNPCAssembly, false, "Optional NPC assembly.");
            Add(g, "Character Model Generator System", CSGeneratorSystemType.CharacterModelGenerator, false, "Optional generated characters.");
            Add(g, "Material / Texture Generator System", CSGeneratorSystemType.MaterialTextureGenerator, true, "Materials/textures.");
        }

        private void Add(CSWorldGenerator g, string name, CSGeneratorSystemType type, bool required, string role)
        {
            g.systems.Add(new CSGeneratorSystemSlot {
                displayName = name,
                type = type,
                required = required,
                useDuringGeneration = true,
                generationRole = role,
                requiredCapability = role
            });
        }
    }
}
#endif
