#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SoulboundLegends.Runtime;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Video;

namespace SoulboundLegends.EditorTools
{
    public sealed class SoulboundLegendsGameBuilder : EditorWindow
    {
        const string SettingsPath = "Assets/SL/Builder/SLBuilderSettings.asset";
        const string SceneFolder = "Assets/Scenes";
        const string ReportPath = "Assets/Documentation/BUILDER_LAST_REPORT.md";
        static readonly string[] SceneNames = {
            "00_Opening", "01_CharacterCreation", "02_Lobby", "03_AdventureWorld",
            "04_Clash", "05_BattleRoyale", "06_Siege", "07_SupremeRuin", "08_Rift"
        };
        static readonly string[] SceneTitles = {
            "SOULBOUND LEGENDS", "CREATE YOUR LEGEND", "THE REALM AWAITS", "ADVENTURE WORLD — VILLAGE 001",
            "CLASH / CS", "BATTLE ROYALE", "SIEGE — CRYSTAL CORE", "SUPREME RUIN", "RIFT BREACH"
        };
        SLBuilderSettings settings;
        Vector2 scroll;
        string lastReport = "Ready. Build Game to generate the scenes and their connected UI.";
        bool isBuilding;

        [MenuItem("Soulbound Legends/Game Builder", priority = 0)]
        public static void Open() { var w = GetWindow<SoulboundLegendsGameBuilder>("Soulbound Legends Builder"); w.minSize = new Vector2(440, 620); w.Show(); }

        void OnEnable() { LoadOrCreateSettings(); }
        void LoadOrCreateSettings()
        {
            settings = AssetDatabase.LoadAssetAtPath<SLBuilderSettings>(SettingsPath);
            if (settings != null) return;
            EnsureFolder("Assets/SL"); EnsureFolder("Assets/SL/Builder");
            settings = CreateInstance<SLBuilderSettings>();
            AssetDatabase.CreateAsset(settings, SettingsPath);
            AssetDatabase.SaveAssets();
        }

        void OnGUI()
        {
            if (settings == null) LoadOrCreateSettings();
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("SOULBOUND LEGENDS", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Automatic scene, UI, world and system assembly", EditorStyles.wordWrappedLabel);
            EditorGUILayout.HelpBox("Build Game creates and saves the nine Unity scenes, generates their world geometry and UI, connects navigation buttons, registers scenes in Build Settings, and writes a validation report. It does not replace premium source assets or configure external online services automatically.", MessageType.Info);
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("OPENING CINEMATIC", EditorStyles.boldLabel);
            settings.introVideo = (VideoClip)EditorGUILayout.ObjectField("Intro Video (optional)", settings.introVideo, typeof(VideoClip), false);
            settings.autoplayIntro = EditorGUILayout.Toggle("Autoplay Intro", settings.autoplayIntro);
            settings.allowSkipIntro = EditorGUILayout.Toggle("Allow Skip Button", settings.allowSkipIntro);
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("GENERATION SETTINGS", EditorStyles.boldLabel);
            settings.worldSeed = EditorGUILayout.IntField("World Seed", settings.worldSeed);
            settings.vegetationCount = EditorGUILayout.IntSlider("Vegetation Count", settings.vegetationCount, 4, 80);
            settings.monsterCount = EditorGUILayout.IntSlider("Monster Count", settings.monsterCount, 0, 30);
            settings.includeProceduralActors = EditorGUILayout.Toggle("Generate Characters / Creatures", settings.includeProceduralActors);
            settings.generateUI = EditorGUILayout.Toggle("Generate UI Panels and Buttons", settings.generateUI);
            settings.addThirdPersonCamera = EditorGUILayout.Toggle("Third-Person Camera", settings.addThirdPersonCamera);
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("ADVANCED VISUAL QUALITY", EditorStyles.boldLabel);
            settings.generateComplexTerrain = EditorGUILayout.Toggle("Generate Complex Terrain Mesh", settings.generateComplexTerrain);
            settings.terrainResolution = EditorGUILayout.IntSlider("Terrain Mesh Resolution", settings.terrainResolution, 33, 161);
            settings.terrainRelief = EditorGUILayout.Slider("Terrain Relief", settings.terrainRelief, .5f, 5f);
            settings.highDetailCharacterMeshes = EditorGUILayout.Toggle("High-Detail Character Surfaces", settings.highDetailCharacterMeshes);
            settings.applyPerformanceDefaults = EditorGUILayout.Toggle("Mobile Performance Defaults", settings.applyPerformanceDefaults);
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("ANDROID BUILD", EditorStyles.boldLabel);
            settings.androidOutputPath = EditorGUILayout.TextField("APK Output Path", settings.androidOutputPath);
            settings.developmentBuild = EditorGUILayout.Toggle("Development Build", settings.developmentBuild);
            EditorUtility.SetDirty(settings);
            EditorGUILayout.Space(12);
            using (new EditorGUI.DisabledScope(isBuilding))
            {
                GUI.backgroundColor = new Color(.35f, .72f, .95f);
                if (GUILayout.Button(isBuilding ? "BUILDING..." : "BUILD GAME", GUILayout.Height(48))) BuildGame();
                GUI.backgroundColor = Color.white;
                if (GUILayout.Button("VALIDATE GENERATED PROJECT", GUILayout.Height(32))) ValidateProject(true);
                if (GUILayout.Button("BUILD ANDROID APK", GUILayout.Height(36))) BuildAndroidApk();
            }
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("LATEST BUILDER REPORT", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(lastReport, MessageType.None);
            EditorGUILayout.EndScrollView();
        }

        /// <summary>Called by the Unity build preprocessor so cloud/local builds do not package stale placeholder scenes.</summary>
        public static void BuildScenesForAutomatedBuild()
        {
            var window = CreateInstance<SoulboundLegendsGameBuilder>();
            try
            {
                window.LoadOrCreateSettings();
                EnsureFolder(SceneFolder); EnsureFolder("Assets/Documentation"); EnsureFolder("Assets/SL/Builder/GeneratedMaterials");
                var generated = new List<string>();
                for (int i = 0; i < SceneNames.Length; i++)
                {
                    window.GenerateScene(i);
                    generated.Add(SceneFolder + "/" + SceneNames[i] + ".unity");
                }
                window.RegisterBuildScenes(generated);
                AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
                window.WriteReport("# Soulbound Legends Build Automation\n\nGenerated all nine scenes automatically before the Unity player build.\n\n" +
                    "Procedural character meshes, layered clothing, preview models and motion primitives are assembled by the generation pipeline.\n" +
                    "This is an automated asset-generation pass, not proof of runtime, device or multiplayer certification.");
                Debug.Log("Soulbound Legends: generated nine scenes before Unity player build.");
            }
            finally { DestroyImmediate(window); }
        }

        void BuildGame()
        {
            if (isBuilding) return;
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            isBuilding = true;
            try
            {
                LoadOrCreateSettings();
                EnsureFolder(SceneFolder); EnsureFolder("Assets/Documentation"); EnsureFolder("Assets/SL/Builder/GeneratedMaterials");
                var generated = new List<string>();
                for (int i = 0; i < SceneNames.Length; i++)
                {
                    EditorUtility.DisplayProgressBar("Soulbound Legends Builder", "Generating " + SceneTitles[i], (float)i / SceneNames.Length);
                    GenerateScene(i);
                    generated.Add(SceneFolder + "/" + SceneNames[i] + ".unity");
                }
                RegisterBuildScenes(generated);
                AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
                EditorSceneManager.OpenScene(SceneFolder + "/00_Opening.unity", OpenSceneMode.Single);
                lastReport = ValidateProject(false);
                EditorSceneManager.OpenScene(SceneFolder + "/00_Opening.unity", OpenSceneMode.Single);
                WriteReport(lastReport);
                EditorUtility.DisplayDialog("Soulbound Legends Builder", "Build Game finished its editor generation pass. Review the report for warnings and external requirements. This is not a claim that the game has passed runtime or multiplayer tests.", "OK");
            }
            catch (Exception e)
            {
                lastReport = "BUILD FAILED: " + e.Message + "\n\n" + e.StackTrace;
                WriteReport(lastReport);
                Debug.LogException(e);
                EditorUtility.DisplayDialog("Soulbound Legends Builder — Error", e.Message + "\nSee Console and BUILDER_LAST_REPORT.md for details.", "OK");
            }
            finally { EditorUtility.ClearProgressBar(); isBuilding = false; Repaint(); }
        }

        void GenerateScene(int index)
        {
            SLProceduralActorFactory.UseHighDetailMeshes = settings.highDetailCharacterMeshes;
            string path = SceneFolder + "/" + SceneNames[index] + ".unity";
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = SceneNames[index];
            UnityEngine.Random.InitState(settings.worldSeed + index * 137);
            var root = new GameObject("SL_GeneratedSceneRoot");
            if (settings.applyPerformanceDefaults) root.AddComponent<SLRuntimeQualityBootstrap>();
            var controller = root.AddComponent<SLRuntimeSceneController>();
            if (index == 3) { root.AddComponent<SLVillageGameplaySystems>(); root.AddComponent<SLAdventureWorldDirector>(); root.AddComponent<SLQuestProgression>(); }
            controller.sceneKind = (SLSceneKind)index;
            controller.sceneDisplayName = SceneTitles[index];
            controller.seed = settings.worldSeed + index * 137;
            controller.buildRuntimeWorld = false;
            controller.buildHud = false;
            controller.allowSceneNavigation = false;
            controller.introVideo = index == 0 ? settings.introVideo : null;
            controller.playOpeningVideo = settings.autoplayIntro;

            ConfigureEnvironment(index);
            var cameraGo = new GameObject("Main Camera");
            cameraGo.tag = "MainCamera";
            cameraGo.transform.position = new Vector3(0, 4.5f, -8f);
            var camera = cameraGo.AddComponent<Camera>(); camera.fieldOfView = 60; camera.nearClipPlane = .05f; camera.farClipPlane = 1000;
            cameraGo.AddComponent<AudioListener>();
            if (index == 0 && settings.introVideo != null)
            {
                var vp = cameraGo.AddComponent<VideoPlayer>(); vp.playOnAwake = false; vp.isLooping = false; vp.source = VideoSource.VideoClip; vp.clip = settings.introVideo;
                vp.renderMode = VideoRenderMode.CameraNearPlane; vp.targetCamera = camera; vp.audioOutputMode = VideoAudioOutputMode.Direct;
            }
            var sun = new GameObject("SL_Sunlight"); var light = sun.AddComponent<Light>(); light.type = LightType.Directional; light.intensity = 1.25f; light.shadows = LightShadows.Soft; light.shadowStrength = .82f; light.shadowBias = .045f; light.shadowNormalBias = .35f; light.shadowResolution = LightShadowResolution.Medium; sun.transform.rotation = Quaternion.Euler(48, -32, 0);
            if (settings.applyPerformanceDefaults) { QualitySettings.shadowDistance = 65f; QualitySettings.shadowCascades = 2; QualitySettings.antiAliasing = 2; QualitySettings.lodBias = 1.25f; QualitySettings.maximumLODLevel = 0; }
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(.28f,.36f,.52f); RenderSettings.ambientEquatorColor = new Color(.22f,.25f,.3f); RenderSettings.ambientGroundColor = new Color(.08f,.1f,.08f);
            RenderSettings.fog = index == 7 || index == 8; RenderSettings.fogColor = index == 8 ? new Color(.18f,.025f,.09f) : new Color(.45f,.58f,.68f); RenderSettings.fogDensity = .006f;

            BuildGround(index);
            BuildEnvironmentContent(index);
            Transform player = null;
            if (settings.includeProceduralActors && index != 0)
            {
                Vector3 playerSpawn = index == 1 ? new Vector3(0, .05f, 0) : new Vector3(0, .05f, -4f);
                var playerObject = CreateActor(root.transform, "Player_RiggedProcedural", playerSpawn, SLProceduralActorFactory.ActorStyle.Player, index % 10);
                player = playerObject.transform;
                playerObject.AddComponent<SLGeneratedProgression>();
                if (index == 1)
                {
                    // Character-creation preview: three actual generated, clothed 3D character models.
                    playerObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                    playerObject.transform.localScale = Vector3.one * .92f;
                    var previewA = CreateActor(root.transform, "CharacterPreview_ElementalA", new Vector3(-2.45f, .05f, .25f), SLProceduralActorFactory.ActorStyle.Player, 1);
                    previewA.transform.rotation = Quaternion.Euler(0, 180, 0); previewA.transform.localScale = Vector3.one * .78f;
                    var previewB = CreateActor(root.transform, "CharacterPreview_ElementalB", new Vector3(2.45f, .05f, .25f), SLProceduralActorFactory.ActorStyle.Player, 5);
                    previewB.transform.rotation = Quaternion.Euler(0, 180, 0); previewB.transform.localScale = Vector3.one * .78f;
                    AddCharacterPreviewLabel("FIRE INHERITANCE", new Vector3(-2.45f, 2.85f, .25f), new Color(1f,.35f,.12f));
                    AddCharacterPreviewLabel("NATURE INHERITANCE", new Vector3(2.45f, 2.85f, .25f), new Color(.2f,1f,.55f));
                }
                if (index >= 3) playerObject.AddComponent<SLThirdPersonPlayer>();
                if (index == 3) playerObject.AddComponent<SLStarterWeaponGenerator>();
            }
            if (index == 1)
            {
                // Fixed showroom camera so the generated models are visible from the front.
                cameraGo.transform.position = new Vector3(0, 2.1f, -10.5f);
                cameraGo.transform.LookAt(new Vector3(0, 1.35f, 0));
                camera.fieldOfView = 43;
            }
            else if (player != null && settings.addThirdPersonCamera && index != 0)
            {
                var follow = cameraGo.AddComponent<SLBuilderThirdPersonCamera>(); follow.target = player; follow.distance = 5.2f; follow.height = 1.7f; follow.shoulderOffset = .45f;
                cameraGo.transform.position = player.position + new Vector3(.45f, 2.2f, -5.2f); cameraGo.transform.LookAt(player.position + Vector3.up * 1.7f);
            }
            else cameraGo.transform.LookAt(new Vector3(0, 1.2f, index == 0 ? 8 : 4));
            if (settings.generateUI) BuildUI(index, cameraGo, root.transform);
            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene, path)) throw new IOException("Could not save scene: " + path);
        }

        void ConfigureEnvironment(int index)
        {
            switch (index)
            {
                case 0: RenderSettings.fog = true; RenderSettings.fogColor = new Color(.015f,.025f,.07f); RenderSettings.fogDensity = .014f; break;
                case 2: RenderSettings.fogColor = new Color(.12f,.18f,.27f); break;
                case 3: RenderSettings.fogColor = new Color(.25f,.46f,.47f); break;
                case 7: RenderSettings.fogColor = new Color(.1f,.05f,.17f); break;
                case 8: RenderSettings.fogColor = new Color(.18f,.025f,.08f); break;
            }
        }

        void BuildGround(int index)
        {
            Color grass = index == 5 ? new Color(.26f,.28f,.2f) : index == 7 ? new Color(.14f,.1f,.2f) : index == 8 ? new Color(.16f,.055f,.09f) : new Color(.13f,.3f,.17f);
            if (settings.generateComplexTerrain) MakeComplexTerrain(index, grass);
            else MakePrimitive("Ground_Terrain", PrimitiveType.Plane, Vector3.zero, new Vector3(index == 5 ? 12 : 8,1,index == 5 ? 12 : 8), grass);
            if (index == 3 || index == 2) MakePrimitive("Village_MainRoad", PrimitiveType.Cube, new Vector3(0,.03f,4), new Vector3(4,.06f,45), new Color(.28f,.23f,.17f));
            if (index == 0)
            {
                for (int i=0;i<11;i++)
                {
                    float a = i * Mathf.PI * 2f / 11f; Vector3 p = new Vector3(Mathf.Cos(a)*8f, 1.2f, 8f + Mathf.Sin(a)*5f);
                    MakePrimitive("God_Throne_"+(i+1), PrimitiveType.Cube, p, new Vector3(1.4f,2.6f,1.1f), new Color(.2f,.25f,.4f));
                    MakePrimitive("God_Orb_"+(i+1), PrimitiveType.Sphere, p + Vector3.up*2f, Vector3.one*.65f, ElementColor(i));
                    MakeVFX("God_Aura_"+(i+1), p + Vector3.up*2f, ElementColor(i), .18f, 5f);
                }
                MakePrimitive("CloudBarrier", PrimitiveType.Sphere, new Vector3(0,10,8), new Vector3(18,4,8), new Color(.48f,.58f,.78f));
            }
            if (index == 1) { MakePrimitive("CharacterCreation_Platform", PrimitiveType.Cylinder, new Vector3(0,.22f,0), new Vector3(3.8f,.45f,3.8f), new Color(.22f,.28f,.4f)); MakePrimitive("CharacterCreation_Crystal", PrimitiveType.Cylinder, new Vector3(0,2.4f,5), new Vector3(.8f,2.4f,.8f), new Color(.15f,.65f,1f)); MakeVFX("CharacterCreation_CrystalVFX", new Vector3(0,3.8f,5), new Color(.15f,.65f,1f), .2f, 7f); }
            if (index == 2)
            {
                for (int i=0;i<7;i++)
                {
                    float x = -16 + i*5.2f; MakePrimitive("Lobby_FloatingIsland_"+i, PrimitiveType.Cylinder, new Vector3(x,-1.1f,15 + (i%2)*4), new Vector3(2.8f,.7f,2.8f), new Color(.16f,.27f,.36f));
                    MakePrimitive("Lobby_Tower_"+i, PrimitiveType.Cube, new Vector3(x,2.5f,16+(i%2)*4), new Vector3(1.5f,5+(i%3),1.5f), new Color(.34f,.37f,.48f));
                }
                MakePrimitive("Lobby_Crystal", PrimitiveType.Cylinder, new Vector3(0,4,18), new Vector3(2.2f,8,2.2f), new Color(.15f,.65f,1f));
                MakeVFX("Lobby_CrystalVFX", new Vector3(0,6,18), new Color(.15f,.65f,1f), .24f, 9f);
            }
            if (index == 3)
            {
                for (int i=0;i<settings.vegetationCount;i++)
                {
                    float x = UnityEngine.Random.Range(-28f,28f), z = UnityEngine.Random.Range(0f,34f);
                    MakePrimitive("Tree_Trunk_"+i, PrimitiveType.Cylinder, new Vector3(x,1.25f,z), new Vector3(.28f,1.25f,.28f), new Color(.22f,.12f,.06f));
                    MakePrimitive("Tree_Canopy_"+i, PrimitiveType.Sphere, new Vector3(x,3f,z), new Vector3(1.8f,2.2f,1.8f), new Color(.08f,.34f,.13f));
                }
                MakeBuilding("VillageHall", new Vector3(10,0,12), new Color(.5f,.42f,.29f));
                for (int i=0;i<5;i++) MakeBuilding("VillageHouse_"+i, new Vector3(-12+(i%3)*6,0,5+(i/3)*7), new Color(.48f,.32f,.22f));
                MakePrimitive("CommonArtifact_Pedestal", PrimitiveType.Cylinder, new Vector3(3,.45f,8), new Vector3(.65f,.45f,.65f), new Color(.28f,.28f,.34f));
                MakePrimitive("CommonArtifact_Crystal", PrimitiveType.Sphere, new Vector3(3,1.3f,8), new Vector3(.5f,.8f,.5f), new Color(.35f,.85f,1f));
                MakeVFX("CommonArtifact_Sparkles", new Vector3(3,1.5f,8), new Color(.35f,.85f,1f), .12f, 8f);
            }
            if (index == 4 || index == 5)
            {
                for (int i=0;i<16;i++) MakePrimitive("ArenaCover_"+i, PrimitiveType.Cube, new Vector3(UnityEngine.Random.Range(-22f,22f),1,UnityEngine.Random.Range(0f,28f)), new Vector3(UnityEngine.Random.Range(1.4f,3.5f),2,2), new Color(.25f,.27f,.32f));
            }
            if (index == 6)
            {
                MakePrimitive("Siege_Wall", PrimitiveType.Cube, new Vector3(0,3,22), new Vector3(36,6,2), new Color(.31f,.31f,.36f));
                MakePrimitive("Siege_CrystalCore", PrimitiveType.Cylinder, new Vector3(0,1.5f,19), new Vector3(1.6f,3,1.6f), new Color(.08f,.72f,1f));
                MakeVFX("Siege_CoreVFX", new Vector3(0,2,19), new Color(.08f,.72f,1f), .22f, 10f);
                for (int i=0;i<6;i++) MakePrimitive("Siege_Tower_"+i, PrimitiveType.Cube, new Vector3(-16+i*6.4f,3,21), new Vector3(2,6,2), new Color(.36f,.34f,.39f));
            }
            if (index == 7)
            {
                MakePrimitive("SupremeRuin_Altar", PrimitiveType.Cylinder, new Vector3(0,.5f,12), new Vector3(6,1,6), new Color(.28f,.18f,.4f));
                for (int i=0;i<5;i++) { MakePrimitive("DivinePillar_"+i, PrimitiveType.Cylinder, new Vector3(-10+i*5,3,19), new Vector3(1,3,1), ElementColor(i)); MakeVFX("DivinePillarVFX_"+i, new Vector3(-10+i*5,4.2f,19), ElementColor(i), .18f, 5f); }
            }
            if (index == 8)
            {
                MakePrimitive("RiftPortal_Frame", PrimitiveType.Cylinder, new Vector3(0,3,17), new Vector3(4,.4f,4), new Color(.65f,.04f,.22f));
                MakePrimitive("RiftPortal_Core", PrimitiveType.Sphere, new Vector3(0,4,17), new Vector3(3.4f,4.5f,.5f), new Color(.5f,.04f,.3f));
                MakeVFX("RiftPortalVFX", new Vector3(0,4,17), new Color(.85f,.04f,.24f), .3f, 12f);
            }
        }

        void BuildEnvironmentContent(int index)
        {
            if (!settings.includeProceduralActors) return;
            if (index == 3)
            {
                for (int i=0;i<4;i++)
                {
                    var npc = CreateActor(SceneManager.GetActiveScene().GetRootGameObjects().First(x=>x.name=="SL_GeneratedSceneRoot").transform, "Village_NPC_"+i, new Vector3(-6+i*4,.05f,8+(i%2)*4), SLProceduralActorFactory.ActorStyle.Villager, i);
                    var interaction = npc.GetComponent<SLVillageNPC>(); if (interaction == null) interaction = npc.AddComponent<SLVillageNPC>();
                    interaction.npcName = i == 0 ? "Adventure Guild Guide" : i == 1 ? "Village Alchemist" : i == 2 ? "Village Blacksmith" : "Village Resident";
                    interaction.greeting = i == 0 ? "Welcome to Village 001. Accept your first quest and explore safely." : i == 1 ? "Bring herbs and water to brew healing potions." : i == 2 ? "Bring ore, wood and monster cores to craft weapons." : "The forest trail holds resources and dangerous beasts.";
                }
                for (int i=0;i<Mathf.Min(settings.monsterCount,8);i++)
                {
                    var beast = CreateActor(SceneManager.GetActiveScene().GetRootGameObjects().First(x=>x.name=="SL_GeneratedSceneRoot").transform, "ElementalBeast_"+i, new Vector3(-18+(i%4)*9,.05f,19+(i/4)*6), SLProceduralActorFactory.ActorStyle.Beast, i);
                    if (beast.GetComponent<SLMonsterBrain>() == null) beast.AddComponent<SLMonsterBrain>();
                }
            }
            else if (index == 4 || index == 5 || index == 6 || index == 8)
            {
                int count = index == 6 ? Mathf.Min(settings.monsterCount,12) : Mathf.Min(settings.monsterCount,10);
                var root = SceneManager.GetActiveScene().GetRootGameObjects().First(x=>x.name=="SL_GeneratedSceneRoot").transform;
                for (int i=0;i<count;i++) CreateActor(root, index == 8 && i == count-1 ? "RiftBoss" : "ArenaEnemy_"+i, new Vector3(UnityEngine.Random.Range(-16f,16f),.05f,UnityEngine.Random.Range(7f,25f)), index == 8 && i == count-1 ? SLProceduralActorFactory.ActorStyle.DivineBoss : SLProceduralActorFactory.ActorStyle.Beast, i);
            }
            else if (index == 7)
            {
                var root = SceneManager.GetActiveScene().GetRootGameObjects().First(x=>x.name=="SL_GeneratedSceneRoot").transform;
                for (int i=0;i<5;i++) CreateActor(root, "DivineCreature_"+(i+1), new Vector3(-10+i*5,.05f,20), SLProceduralActorFactory.ActorStyle.DivineBoss, i);
            }
        }

        void BuildUI(int index, GameObject cameraGo, Transform sceneRoot)
        {
            if (UnityEngine.Object.FindFirstObjectByType<EventSystem>() == null)
            {
                var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
            var canvasGo = new GameObject("SL_GeneratedUI"); var canvas = canvasGo.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1920,1080); scaler.matchWidthOrHeight = .5f; canvasGo.AddComponent<GraphicRaycaster>();
            MakePanel(canvas.transform, "TopBar", new Vector2(.5f,1), new Vector2(0,-52), new Vector2(1500,82), new Color(.025f,.045f,.09f,.82f));
            var title = MakeText(canvas.transform, "Title", SceneTitles[index], new Vector2(.5f,1), new Vector2(0,-48), new Vector2(1100,58), 28, new Color(.88f,.91f,1f)); title.alignment = TextAnchor.MiddleCenter;
            if (index == 0)
            {
                MakeText(canvas.transform,"OpeningSubtitle","ELEVEN INHERITANCES. ONE HIDDEN SHADOW.",new Vector2(.5f,.25f),new Vector2(0,0),new Vector2(900,54),20,new Color(.62f,.82f,1f)).alignment=TextAnchor.MiddleCenter;
                MakeButton(canvas.transform,"CONTINUE",new Vector2(.5f,.1f),new Vector2(0,85),new Vector2(260,68),null,"Panel_Login");
                if (settings.allowSkipIntro) MakeText(canvas.transform,"SkipHint","Continue after the cinematic",new Vector2(.5f,.1f),new Vector2(0,35),new Vector2(500,28),14,new Color(.75f,.8f,.9f)).alignment=TextAnchor.MiddleCenter;
                MakePanel(canvas.transform,"Panel_Login",new Vector2(.5f,.5f),new Vector2(0,0),new Vector2(620,500),new Color(.025f,.045f,.09f,.97f),false);
                MakeText(canvas.transform.Find("Panel_Login"),"LoginTitle","SIGN IN TO SOULBOUND LEGENDS",new Vector2(.5f,.5f),new Vector2(0,155),new Vector2(520,60),25,Color.white).alignment=TextAnchor.MiddleCenter;
                MakeButton(canvas.transform.Find("Panel_Login"),"GOOGLE SIGN IN",new Vector2(.5f,.5f),new Vector2(0,75),new Vector2(330,58),null,null,null,"Google sign-in is not active until provider credentials are configured.");
                MakeButton(canvas.transform.Find("Panel_Login"),"EMAIL SIGN IN",new Vector2(.5f,.5f),new Vector2(0,0),new Vector2(330,58),null,null,null,"Email sign-in requires the authentication provider to be configured.");
                MakeButton(canvas.transform.Find("Panel_Login"),"GUEST START (TESTING)",new Vector2(.5f,.5f),new Vector2(0,-85),new Vector2(330,58),"01_CharacterCreation");
                MakeText(canvas.transform.Find("Panel_Login"),"LoginStatus","Online sign-in requires configured credentials. Guest start is for local testing.",new Vector2(.5f,.5f),new Vector2(0,-155),new Vector2(500,70),16,new Color(.82f,.86f,.94f)).alignment=TextAnchor.MiddleCenter;
            }
            else if (index == 1)
            {
                MakePanel(canvas.transform,"CharacterPanel",new Vector2(0, .5f),new Vector2(210,0),new Vector2(360,520),new Color(.025f,.045f,.09f,.82f));
                MakeText(canvas.transform,"CharacterInstructions","CHOOSE YOUR LOOK\nFace • Hair • Outfit • Element",new Vector2(0,.5f),new Vector2(210,160),new Vector2(330,100),20,Color.white).alignment=TextAnchor.MiddleCenter;
                MakeButton(canvas.transform,"CHANGE ELEMENT",new Vector2(0,.5f),new Vector2(210,40),new Vector2(270,48),null,null,"ChangeElement");
                MakeButton(canvas.transform,"RANDOMIZE LOOK",new Vector2(0,.5f),new Vector2(210,-20),new Vector2(270,48),null,null,"RandomizeLook");
                MakeButton(canvas.transform,"ENTER LOBBY",new Vector2(.5f,.08f),Vector2.zero,new Vector2(300,68),"02_Lobby");
            }
            else if (index == 2)
            {
                MakeText(canvas.transform,"LobbyTagline","Choose your path. Discover your inheritance.",new Vector2(.5f,.84f),Vector2.zero,new Vector2(900,44),20,new Color(.8f,.88f,1f)).alignment=TextAnchor.MiddleCenter;
                MakeButton(canvas.transform,"ADVENTURE WORLD",new Vector2(.5f,.12f),new Vector2(-390,0),new Vector2(250,70),"03_AdventureWorld");
                MakeButton(canvas.transform,"CLASH / CS",new Vector2(.5f,.12f),new Vector2(-130,0),new Vector2(220,70),"04_Clash");
                MakeButton(canvas.transform,"BATTLE ROYALE",new Vector2(.5f,.12f),new Vector2(110,0),new Vector2(240,70),"05_BattleRoyale");
                MakeButton(canvas.transform,"SIEGE",new Vector2(.5f,.12f),new Vector2(380,0),new Vector2(200,70),"06_Siege");
                MakeText(canvas.transform,"UnlockNotice","CS • BR • SIEGE unlock at Level 10",new Vector2(.5f,.06f),new Vector2(0,-25),new Vector2(750,32),16,new Color(.95f,.8f,.45f)).alignment=TextAnchor.MiddleCenter;
                AddSideMenu(canvas.transform);
            }
            else
            {
                MakeText(canvas.transform,"StatsHUD","HP 1000     LV 1     XP 0 / 100",new Vector2(0,0),new Vector2(210,45),new Vector2(420,50),22,Color.white);
                MakeText(canvas.transform,"QuestHUD","QUEST  •  Explore the area",new Vector2(1,1),new Vector2(-210,-130),new Vector2(380,70),18,new Color(.95f,.88f,.66f)).alignment=TextAnchor.UpperRight;
                if (index >= 3) MakeText(canvas.transform,"SL_StatusText","Ready. Explore, gather resources, speak to NPCs and craft.",new Vector2(.5f,0),new Vector2(0,28),new Vector2(900,42),16,new Color(.72f,.9f,1f)).alignment=TextAnchor.MiddleCenter;
                MakeButton(canvas.transform,"LOBBY",new Vector2(1,0),new Vector2(-120,55),new Vector2(170,56),"02_Lobby");
                MakeButton(canvas.transform,"QUESTS",new Vector2(0,1),new Vector2(100,-135),new Vector2(170,50),null,"Panel_Quests");
                MakeButton(canvas.transform,"INVENTORY",new Vector2(0,1),new Vector2(100,-195),new Vector2(170,50),null,"Panel_Inventory");
                MakeButton(canvas.transform,"MAP",new Vector2(0,1),new Vector2(100,-255),new Vector2(170,50),null,"Panel_Map");
                if (index == 3)
                {
                    MakeButton(canvas.transform,"GATHER",new Vector2(0,1),new Vector2(100,-315),new Vector2(170,48),null,null,"GatherMaterials");
                    MakeButton(canvas.transform,"BLACKSMITH",new Vector2(0,1),new Vector2(100,-370),new Vector2(170,48),null,"Panel_Blacksmith");
                    MakeButton(canvas.transform,"ALCHEMY",new Vector2(0,1),new Vector2(100,-425),new Vector2(170,48),null,"Panel_Alchemy");
                    MakeButton(canvas.transform,"TALK TO GUILD",new Vector2(0,1),new Vector2(100,-480),new Vector2(170,48),null,null,"GuildQuest");
                    MakeButton(canvas.transform,"INTERACT / GATHER NEARBY",new Vector2(0,1),new Vector2(100,-535),new Vector2(220,48),null,null,"InteractNearest");
                    MakeButton(canvas.transform,"USE HEALING POTION",new Vector2(1,0),new Vector2(-250,55),new Vector2(200,50),null,null,"UsePotion");
                    MakeButton(canvas.transform,"SAVE PROGRESS",new Vector2(1,0),new Vector2(-120,55),new Vector2(170,50),null,null,"SaveProgress");
                    MakePanel(canvas.transform,"Panel_Blacksmith",new Vector2(.5f,.5f),Vector2.zero,new Vector2(650,480),new Color(.025f,.045f,.09f,.97f),false);
                    MakeText(canvas.transform.Find("Panel_Blacksmith"),"BlacksmithText","VILLAGE BLACKSMITH\nForge beginner weapons using gathered materials.\nIron Sword: 3 Iron Ore + 1 Wood\nApprentice Staff: 3 Wood + 1 Monster Core\nHunter Bow: 3 Wood + 1 Herb",new Vector2(.5f,.5f),new Vector2(0,100),new Vector2(560,220),20,Color.white);
                    MakeButton(canvas.transform.Find("Panel_Blacksmith"),"FORGE IRON SWORD",new Vector2(.5f,.5f),new Vector2(-150,-65),new Vector2(230,48),null,null,"ForgeSword");
                    MakeButton(canvas.transform.Find("Panel_Blacksmith"),"FORGE STAFF",new Vector2(.5f,.5f),new Vector2(100,-65),new Vector2(210,48),null,null,"ForgeStaff");
                    MakeButton(canvas.transform.Find("Panel_Blacksmith"),"CRAFT BOW",new Vector2(.5f,.5f),new Vector2(0,-125),new Vector2(210,48),null,null,"ForgeBow");
                    MakePanel(canvas.transform,"Panel_Alchemy",new Vector2(.5f,.5f),Vector2.zero,new Vector2(650,430),new Color(.025f,.045f,.09f,.97f),false);
                    MakeText(canvas.transform.Find("Panel_Alchemy"),"AlchemyText","VILLAGE ALCHEMY\nBrew a Minor Healing Potion.\nRecipe: 2 Herbs + 1 Water Flask",new Vector2(.5f,.5f),new Vector2(0,70),new Vector2(560,170),22,Color.white);
                    MakeButton(canvas.transform.Find("Panel_Alchemy"),"BREW HEALING POTION",new Vector2(.5f,.5f),new Vector2(0,-65),new Vector2(300,52),null,null,"CraftPotion");
                }
                MakeButton(canvas.transform,"DODGE",new Vector2(1,0),new Vector2(-410,125),new Vector2(135,58),null,null,"Dodge");
                MakeButton(canvas.transform,"ATTACK",new Vector2(1,0),new Vector2(-265,125),new Vector2(145,64),null,null,"Attack");
                MakeButton(canvas.transform,"BLOCK",new Vector2(1,0),new Vector2(-115,125),new Vector2(135,58),null,null,"Block");
                MakeButton(canvas.transform,"HEAVY ATTACK",new Vector2(1,0),new Vector2(-265,205),new Vector2(175,58),null,null,"HeavyAttack");
                MakeButton(canvas.transform,"ELEMENTAL SKILL",new Vector2(1,0),new Vector2(-90,205),new Vector2(190,64),null,null,"ElementalSkill");
                if (index >= 3) MakeJoystick(canvas.transform);
                MakePanel(canvas.transform,"Panel_Quests",new Vector2(.5f,.5f),Vector2.zero,new Vector2(650,450),new Color(.025f,.045f,.09f,.96f),false);
                MakeText(canvas.transform.Find("Panel_Quests"),"QuestPanelText","QUEST LOG\n\n• Explore the village\n• Defeat an elemental beast\n• Discover a common artifact\n• Return to the Adventure Guild",new Vector2(.5f,.5f),new Vector2(0,35),new Vector2(570,300),23,Color.white).alignment=TextAnchor.MiddleLeft;
                MakeButton(canvas.transform.Find("Panel_Quests"),"CLAIM QUEST REWARD",new Vector2(.5f,0),new Vector2(0,52),new Vector2(280,48),null,null,"ClaimGuildQuest");
                MakePanel(canvas.transform,"Panel_Inventory",new Vector2(.5f,.5f),Vector2.zero,new Vector2(650,450),new Color(.025f,.045f,.09f,.96f),false);
                MakeText(canvas.transform.Find("Panel_Inventory"),"InventoryPanelText","INVENTORY\n\nCommon Artifact\nMaterials\nEquipment\nConsumables",new Vector2(.5f,.5f),Vector2.zero,new Vector2(560,260),23,Color.white).alignment=TextAnchor.MiddleLeft;
                MakePanel(canvas.transform,"Panel_Map",new Vector2(.5f,.5f),Vector2.zero,new Vector2(650,450),new Color(.025f,.045f,.09f,.96f),false);
                MakeText(canvas.transform.Find("Panel_Map"),"MapPanelText","WORLD MAP\n\nVillage 001  •  Adventure Guild\nForest Trail  •  Monster Grounds\nKingdom Portal (locked until Level 10)",new Vector2(.5f,.5f),Vector2.zero,new Vector2(560,260),23,Color.white).alignment=TextAnchor.MiddleLeft;
                if (index == 3)
                {
                    MakeButton(canvas.transform.Find("Panel_Inventory"),"REFRESH INVENTORY",new Vector2(.5f,.5f),new Vector2(0,-125),new Vector2(250,44),null,null,"ShowInventory");
                    MakeButton(canvas.transform.Find("Panel_Inventory"),"EQUIP SWORD",new Vector2(.5f,.5f),new Vector2(-165,-175),new Vector2(180,44),null,null,"EquipSword");
                    MakeButton(canvas.transform.Find("Panel_Inventory"),"EQUIP STAFF",new Vector2(.5f,.5f),new Vector2(0,-175),new Vector2(180,44),null,null,"EquipStaff");
                    MakeButton(canvas.transform.Find("Panel_Inventory"),"EQUIP BOW",new Vector2(.5f,.5f),new Vector2(165,-175),new Vector2(180,44),null,null,"EquipBow");
                    MakeButton(canvas.transform.Find("Panel_Quests"),"CLAIM GUILD QUEST",new Vector2(.5f,.5f),new Vector2(0,-125),new Vector2(250,44),null,null,"ClaimGuildQuest");
                }
                MakeText(canvas.transform,"LevelUpNotice","LEVEL UP!",new Vector2(.5f,.72f),Vector2.zero,new Vector2(520,70),34,new Color(1f,.82f,.3f)).alignment=TextAnchor.MiddleCenter;
                AddPanelCloseButtons(canvas.transform);
            }
            EditorUtility.SetDirty(canvasGo);
        }

        void AddSideMenu(Transform canvas)
        {
            string[] labels = { "PROFILE", "EVENTS", "FREE ITEMS", "LUCK ROYALE", "STORE / AETHER", "VAULT", "ARMORY", "EMOTES", "FRIENDS", "WORLD CHAT", "SETTINGS", "MAIL" };
            string[] descriptions = {
                "PROFILE\n\nCharacter level, element, combat stats and progression overview.",
                "EVENTS\n\nLive events and seasonal challenges appear here when content is configured.",
                "FREE ITEMS\n\nClaimable free rewards will be listed here when the reward catalogue is configured.",
                "LUCK ROYALE\n\nTicket Draw • Fate Vault • Aether Draw. Configure reward pools, probabilities, pity and server validation before release.",
                "STORE / AETHER\n\nCosmetics, outfits and Aether balance. Real-money purchases require the configured Google Play billing bridge.",
                "VAULT\n\nOwned cosmetics, artifacts and collectibles.",
                "ARMORY\n\nWeapons and equipment. Beginner weapons are crafted at the village blacksmith.",
                "EMOTES\n\nUnlocked emotes and character expressions.",
                "FRIENDS\n\nAdd players, party invites and friend list require online social service configuration.",
                "WORLD CHAT\n\nWorld, guild and party chat require a live network/chat provider.",
                "SETTINGS\n\nGraphics, audio, controls and HUD preferences.",
                "MAIL\n\nSystem mail and rewards require account-backed persistence."
            };
            for (int i=0;i<labels.Length;i++)
            {
                int column = i / 6; int row = i % 6;
                MakeButton(canvas,labels[i],new Vector2(1,1),new Vector2(-110-column*190,-145-row*54),new Vector2(174,44),null,"Panel_"+labels[i].Replace(" / ","_").Replace(" ","_"));
                string panelName = "Panel_"+labels[i].Replace(" / ","_").Replace(" ","_");
                MakePanel(canvas,panelName,new Vector2(.5f,.5f),Vector2.zero,new Vector2(620,390),new Color(.025f,.045f,.09f,.97f),false);
                MakeText(canvas.Find(panelName),"PanelText_"+i,descriptions[i],new Vector2(.5f,.5f),Vector2.zero,new Vector2(530,250),22,Color.white).alignment=TextAnchor.MiddleCenter;
            }
            AddPanelCloseButtons(canvas);
        }

        void AddPanelCloseButtons(Transform canvas)
        {
            foreach (Transform child in canvas)
            {
                if (!child.name.StartsWith("Panel_")) continue;
                var panel = child.gameObject;
                var buttonGo = new GameObject("ClosePanelButton"); buttonGo.transform.SetParent(panel.transform,false);
                var rt = buttonGo.AddComponent<RectTransform>(); rt.anchorMin=rt.anchorMax=new Vector2(1,1); rt.anchoredPosition=new Vector2(-48,-36); rt.sizeDelta=new Vector2(72,42);
                var img = buttonGo.AddComponent<Image>(); img.color=new Color(.55f,.12f,.16f,.95f);
                var button = buttonGo.AddComponent<Button>(); var closer = buttonGo.AddComponent<SLGeneratedPanelCloser>(); closer.panel=panel;
                UnityEventTools.AddPersistentListener(button.onClick, closer.Close);
                var label = MakeText(buttonGo.transform,"CloseText","CLOSE",new Vector2(.5f,.5f),Vector2.zero,new Vector2(70,40),13,Color.white); label.alignment=TextAnchor.MiddleCenter;
            }
        }

        GameObject MakePanel(Transform parent,string name,Vector2 anchor,Vector2 position,Vector2 size,Color color,bool active=true)
        {
            var go=new GameObject(name); go.transform.SetParent(parent,false); go.SetActive(active);
            var rt=go.AddComponent<RectTransform>(); rt.anchorMin=rt.anchorMax=anchor; rt.anchoredPosition=position; rt.sizeDelta=size;
            var image=go.AddComponent<Image>(); image.color=color; return go;
        }
        Text MakeText(Transform parent,string name,string value,Vector2 anchor,Vector2 position,Vector2 size,int fontSize,Color color)
        {
            var go=new GameObject(name); go.transform.SetParent(parent,false); var rt=go.AddComponent<RectTransform>(); rt.anchorMin=rt.anchorMax=anchor; rt.anchoredPosition=position; rt.sizeDelta=size;
            var t=go.AddComponent<Text>(); t.text=value; t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); t.fontSize=fontSize; t.color=color; t.horizontalOverflow=HorizontalWrapMode.Wrap; t.verticalOverflow=VerticalWrapMode.Overflow; t.alignment=TextAnchor.MiddleLeft; return t;
        }
        void MakeButton(Transform parent,string label,Vector2 anchor,Vector2 position,Vector2 size,string targetScene,string targetPanel=null,string gameplayCommand=null,string diagnosticMessage=null)
        {
            var go=new GameObject("Button_"+label.Replace('/','_').Replace(' ','_')); go.transform.SetParent(parent,false); var rt=go.AddComponent<RectTransform>(); rt.anchorMin=rt.anchorMax=anchor; rt.anchoredPosition=position; rt.sizeDelta=size;
            var image=go.AddComponent<Image>(); image.color=new Color(.055f,.12f,.22f,.94f); var button=go.AddComponent<Button>(); var colors=button.colors; colors.normalColor=new Color(.12f,.23f,.36f,1); colors.highlightedColor=new Color(.18f,.42f,.62f,1); colors.pressedColor=new Color(.08f,.16f,.28f,1); button.colors=colors;
            var action=go.AddComponent<SLGeneratedUIAction>(); action.targetSceneName=targetScene; action.targetPanelName=targetPanel; action.targetPanel=string.IsNullOrEmpty(targetPanel)?null:parent.Find(targetPanel)?.gameObject; action.gameplayCommand=gameplayCommand; action.diagnosticMessage=diagnosticMessage;
            UnityEventTools.AddPersistentListener(button.onClick, action.Execute);
            var text=MakeText(go.transform,"Label",label,new Vector2(.5f,.5f),Vector2.zero,size- new Vector2(12,8),Mathf.Clamp((int)(size.y*.32f),13,22),Color.white); text.alignment=TextAnchor.MiddleCenter;
        }

        void MakeJoystick(Transform canvas)
        {
            var background=new GameObject("MoveJoystick"); background.transform.SetParent(canvas,false);
            var bg=background.AddComponent<RectTransform>(); bg.anchorMin=bg.anchorMax=new Vector2(0,0); bg.anchoredPosition=new Vector2(130,145); bg.sizeDelta=new Vector2(170,170);
            var image=background.AddComponent<Image>(); image.color=new Color(.06f,.1f,.16f,.48f); image.raycastTarget=true;
            var knobGo=new GameObject("Knob"); knobGo.transform.SetParent(background.transform,false);
            var knob=knobGo.AddComponent<RectTransform>(); knob.anchorMin=knob.anchorMax=new Vector2(.5f,.5f); knob.anchoredPosition=Vector2.zero; knob.sizeDelta=new Vector2(68,68);
            var knobImage=knobGo.AddComponent<Image>(); knobImage.color=new Color(.55f,.72f,.9f,.8f); knobImage.raycastTarget=false;
            var joystick=background.AddComponent<SLVirtualJoystick>(); joystick.background=bg; joystick.knob=knob;
        }

        void MakeVFX(string name,Vector3 position,Color color,float size,float emissionRate)
        {
            var go=new GameObject(name); go.transform.position=position;
            var ps=go.AddComponent<ParticleSystem>();
            var main=ps.main; main.startColor=color; main.startSize=size; main.startLifetime=1.8f; main.startSpeed=1.4f; main.duration=3f; main.loop=true; main.simulationSpace=ParticleSystemSimulationSpace.World;
            var emission=ps.emission; emission.rateOverTime=emissionRate;
            var shape=ps.shape; shape.shapeType=ParticleSystemShapeType.Sphere; shape.radius=.45f;
            var renderer=go.GetComponent<ParticleSystemRenderer>(); if(renderer!=null) renderer.renderMode=ParticleSystemRenderMode.Billboard;
        }

        void MakeBuilding(string name,Vector3 basePosition,Color wallColor)
        {
            // Modular fantasy architecture: foundation, wall mass, corner piers, layered roof, trim, windows and entrance details.
            Color stone = Color.Lerp(wallColor, new Color(.16f,.17f,.22f), .28f);
            Color roof = new Color(.22f,.075f,.065f); Color trim = new Color(.72f,.55f,.28f);
            MakePrimitive(name+"_Foundation",PrimitiveType.Cube,basePosition+new Vector3(0,.22f,0),new Vector3(5.6f,.44f,4.6f),stone);
            MakePrimitive(name+"_Walls",PrimitiveType.Cube,basePosition+new Vector3(0,1.72f,0),new Vector3(5,3,4),wallColor);
            for(int side=-1;side<=1;side+=2) for(int front=-1;front<=1;front+=2)
                MakePrimitive(name+"_CornerPier_"+side+"_"+front,PrimitiveType.Cube,basePosition+new Vector3(side*2.34f,1.8f,front*1.86f),new Vector3(.34f,3.3f,.34f),stone);
            MakePrimitive(name+"_RoofLower",PrimitiveType.Cube,basePosition+new Vector3(0,3.24f,0),new Vector3(6.15f,.32f,5.15f),roof);
            MakePrimitive(name+"_RoofUpper",PrimitiveType.Cube,basePosition+new Vector3(0,3.52f,0),new Vector3(5.55f,.28f,4.55f),Color.Lerp(roof,trim,.12f));
            MakePrimitive(name+"_Ridge",PrimitiveType.Cube,basePosition+new Vector3(0,3.76f,0),new Vector3(5.7f,.13f,.18f),trim);
            MakePrimitive(name+"_DoorFrame_L",PrimitiveType.Cube,basePosition+new Vector3(-.58f,.9f,-2.08f),new Vector3(.16f,1.85f,.18f),trim);
            MakePrimitive(name+"_DoorFrame_R",PrimitiveType.Cube,basePosition+new Vector3(.58f,.9f,-2.08f),new Vector3(.16f,1.85f,.18f),trim);
            MakePrimitive(name+"_DoorLintel",PrimitiveType.Cube,basePosition+new Vector3(0,1.86f,-2.08f),new Vector3(1.35f,.18f,.2f),trim);
            MakePrimitive(name+"_Door",PrimitiveType.Cube,basePosition+new Vector3(0,.78f,-2.03f),new Vector3(.9f,1.55f,.12f),new Color(.09f,.045f,.025f));
            for(int side=-1;side<=1;side+=2){
                MakePrimitive(name+"_WindowRecess_"+side,PrimitiveType.Cube,basePosition+new Vector3(side*1.55f,1.95f,-2.035f),new Vector3(.72f,.92f,.08f),stone);
                MakePrimitive(name+"_WindowGlass_"+side,PrimitiveType.Cube,basePosition+new Vector3(side*1.55f,1.95f,-2.09f),new Vector3(.52f,.7f,.055f),new Color(.12f,.55f,.78f));
                MakePrimitive(name+"_WindowCross_"+side,PrimitiveType.Cube,basePosition+new Vector3(side*1.55f,1.95f,-2.13f),new Vector3(.07f,.76f,.045f),trim);
            }
            MakePrimitive(name+"_EntranceStep",PrimitiveType.Cube,basePosition+new Vector3(0,.12f,-2.38f),new Vector3(1.65f,.24f,.7f),stone);
        }

        void MakeComplexTerrain(int sceneIndex, Color terrainColor)
        {
            int resolution = Mathf.Clamp(settings.terrainResolution, 33, 161);
            float size = sceneIndex == 5 ? 120f : 80f;
            var vertices = new Vector3[resolution * resolution]; var uv = new Vector2[vertices.Length];
            var triangles = new int[(resolution - 1) * (resolution - 1) * 6];
            float relief = settings.terrainRelief * (sceneIndex == 5 ? 1.45f : 1f);
            int seed = settings.worldSeed + sceneIndex * 7919;
            float offX = (seed % 997) * .0137f, offZ = (seed % 613) * .0191f;
            for (int z=0; z<resolution; z++) for(int x=0; x<resolution; x++) {
                float u=x/(float)(resolution-1), v=z/(float)(resolution-1);
                float wx=(u-.5f)*size, wz=(v-.5f)*size;
                float edge=Mathf.SmoothStep(0f,1f,Mathf.Clamp01(Mathf.Max(Mathf.Abs(u-.5f),Mathf.Abs(v-.5f))*.95f));
                float broad=Mathf.PerlinNoise(u*3.2f+offX,v*3.2f+offZ)-.5f;
                float medium=Mathf.PerlinNoise(u*9.5f+offZ,v*9.5f+offX)-.5f;
                float fine=Mathf.PerlinNoise(u*27f+offX*2f,v*27f+offZ*2f)-.5f;
                float ridge=1f-Mathf.Abs(Mathf.PerlinNoise(u*5.2f+offZ,v*5.2f+offX)*2f-1f);
                float height=(broad*relief*1.8f+medium*relief*.7f+fine*relief*.18f+ridge*relief*.35f)*edge;
                // Keep the main spawn and village path reasonably walkable while leaving a varied perimeter.
                float spawnFlatten=1f-Mathf.SmoothStep(5f,17f,new Vector2(wx,wz).magnitude);
                height *= 1f-spawnFlatten*.94f;
                vertices[z*resolution+x]=new Vector3(wx,height,wz); uv[z*resolution+x]=new Vector2(u*8f,v*8f);
            }
            int ti=0; for(int z=0;z<resolution-1;z++) for(int x=0;x<resolution-1;x++) {
                int a=z*resolution+x,b=a+resolution,c=a+1,d=b+1;
                triangles[ti++]=a;triangles[ti++]=b;triangles[ti++]=c;triangles[ti++]=c;triangles[ti++]=b;triangles[ti++]=d;
            }
            var mesh=new Mesh{name="SL_ComplexTerrain_"+sceneIndex}; if(vertices.Length>65000) mesh.indexFormat=UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices=vertices;mesh.uv=uv;mesh.triangles=triangles;mesh.RecalculateNormals();mesh.RecalculateBounds();
            var terrain=new GameObject("Ground_Terrain_ComplexMesh");terrain.transform.position=new Vector3(0,0,0);var filter=terrain.AddComponent<MeshFilter>();filter.sharedMesh=mesh;
            var renderer=terrain.AddComponent<MeshRenderer>();renderer.sharedMaterial=MaterialFor(terrainColor);terrain.isStatic=true;
            var collider=terrain.AddComponent<MeshCollider>();collider.sharedMesh=mesh;
            // Layered geological outcrops add a readable silhouette around the playable area.
            UnityEngine.Random.InitState(seed);
            int outcrops=sceneIndex==5?28:18;
            for(int i=0;i<outcrops;i++){
                float angle=UnityEngine.Random.Range(0f,Mathf.PI*2f);float radius=UnityEngine.Random.Range(size*.34f,size*.48f);
                float x=Mathf.Cos(angle)*radius,z=Mathf.Sin(angle)*radius;
                float h=UnityEngine.Random.Range(1.2f,4.8f);var rock=MakePrimitive("Terrain_Outcrop_"+i,PrimitiveType.Sphere,new Vector3(x,h*.38f,z),new Vector3(UnityEngine.Random.Range(1.2f,3.8f),h,UnityEngine.Random.Range(1.3f,4.2f)),Color.Lerp(terrainColor,new Color(.32f,.3f,.28f),UnityEngine.Random.Range(.18f,.48f)));
                rock.transform.rotation=Quaternion.Euler(UnityEngine.Random.Range(-12f,12f),UnityEngine.Random.Range(0f,180f),UnityEngine.Random.Range(-12f,12f));rock.isStatic=true;
            }
        }
        GameObject MakePrimitive(string name,PrimitiveType type,Vector3 position,Vector3 scale,Color color)
        {
            var go=GameObject.CreatePrimitive(type); go.name=name; go.transform.position=position; go.transform.localScale=scale;
            var renderer=go.GetComponent<Renderer>(); if(renderer) renderer.sharedMaterial=MaterialFor(color); return go;
        }
        readonly Dictionary<Color,Material> MaterialCache = new Dictionary<Color,Material>();
        Material MaterialFor(Color color)
        {
            if(MaterialCache.TryGetValue(color,out var cached) && cached!=null) return cached;
            string token=ColorUtility.ToHtmlStringRGBA(color);
            string path="Assets/SL/Builder/GeneratedMaterials/SL_BuilderMat_"+token+".mat";
            var existing=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(existing!=null){MaterialCache[color]=existing;return existing;}
            var shader=Shader.Find("Universal Render Pipeline/Lit"); if(shader==null) shader=Shader.Find("Standard"); if(shader==null) shader=Shader.Find("Diffuse");
            if(shader==null) throw new InvalidOperationException("No supported Unity material shader is available.");
            var material=new Material(shader){name="SL_BuilderMat_"+token,color=color,enableInstancing=true};
            AssetDatabase.CreateAsset(material,path); MaterialCache[color]=material; return material;
        }
        void AddCharacterPreviewLabel(string label, Vector3 position, Color color)
        {
            var go = new GameObject("PreviewLabel_" + label.Replace(" ", "_"));
            go.transform.position = position;
            go.transform.rotation = Quaternion.Euler(0, 180, 0);
            var text = go.AddComponent<TextMesh>(); text.text = label; text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center; text.fontSize = 42; text.characterSize = .045f; text.color = color;
        }
        GameObject CreateActor(Transform parent,string actorName,Vector3 position,SLProceduralActorFactory.ActorStyle style,int variant)
        {
            var actor=SLProceduralActorFactory.Create(parent,actorName,position,style,variant);
            foreach(var renderer in actor.GetComponentsInChildren<Renderer>(true))
            {
                if(renderer.sharedMaterial!=null) renderer.sharedMaterial=MaterialFor(renderer.sharedMaterial.color);
            }
            return actor;
        }
        static Color ElementColor(int index)
        {
            Color[] colors={new Color(.1f,.7f,1f),new Color(1f,.22f,.08f),new Color(.4f,.7f,.22f),new Color(.55f,.2f,.95f),new Color(1f,.8f,.12f),new Color(.15f,.9f,.65f),Color.white,new Color(.5f,.75f,1f),new Color(.55f,.85f,1f),new Color(.85f,.25f,.95f)};
            return colors[Mathf.Abs(index)%colors.Length];
        }
        void RegisterBuildScenes(List<string> paths)
        {
            var scenes=EditorBuildSettings.scenes.ToList();
            foreach(string path in paths)
            {
                string full=Path.GetFullPath(path);
                if(!scenes.Any(s=>s.path==path)) scenes.Add(new EditorBuildSettingsScene(path,true));
                else { int i=scenes.FindIndex(s=>s.path==path); scenes[i]=new EditorBuildSettingsScene(path,true); }
            }
            EditorBuildSettings.scenes=scenes.ToArray();
        }

        string ValidateProject(bool showDialog)
        {
            var report=new StringBuilder(); int errors=0,warnings=0;
            report.AppendLine("# Soulbound Legends Builder Report"); report.AppendLine("Generated: "+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")); report.AppendLine();
            foreach(string name in SceneNames)
            {
                string path=SceneFolder+"/"+name+".unity"; var asset=AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                if(asset==null){errors++;report.AppendLine("- ERROR: Missing scene `"+path+"`. Run BUILD GAME."); continue;}
                if(!EditorBuildSettings.scenes.Any(s=>s.path==path && s.enabled)){warnings++;report.AppendLine("- WARNING: Scene `"+name+"` is not enabled in Build Settings.");}
                else report.AppendLine("- PASS: Scene asset exists and is enabled: `"+name+"`.");
                Scene checkScene = SceneManager.GetSceneByPath(path);
                bool openedForValidation = false;
                try
                {
                    if (!checkScene.IsValid() || !checkScene.isLoaded) { checkScene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive); openedForValidation = true; }
                    var sceneRoots = checkScene.GetRootGameObjects();
                    bool hasCamera = sceneRoots.Any(r => r.GetComponentInChildren<Camera>(true) != null);
                    bool hasController = sceneRoots.Any(r => r.GetComponentInChildren<SLRuntimeSceneController>(true) != null);
                    bool hasCanvas = sceneRoots.Any(r => r.GetComponentInChildren<Canvas>(true) != null);
                    bool hasEventSystem = sceneRoots.Any(r => r.GetComponentInChildren<EventSystem>(true) != null);
                    if (!hasCamera) { errors++; report.AppendLine("- ERROR: `"+name+"` has no camera."); }
                    if (!hasController) { errors++; report.AppendLine("- ERROR: `"+name+"` has no SL runtime scene controller."); }
                    if (settings.generateUI && !hasCanvas) { errors++; report.AppendLine("- ERROR: `"+name+"` has no generated UI Canvas."); }
                    if (settings.generateUI && !hasEventSystem) { errors++; report.AppendLine("- ERROR: `"+name+"` has no EventSystem for UI buttons."); }
                    if(name=="03_AdventureWorld")
                    {
                        if(!sceneRoots.Any(r=>r.GetComponentInChildren<SLVillageGameplaySystems>(true)!=null)){errors++;report.AppendLine("- ERROR: Adventure World is missing its local inventory/crafting system.");}
                        else report.AppendLine("- PASS: Adventure World inventory, blacksmith and alchemy foundation is attached.");
                        if(!sceneRoots.Any(r=>r.GetComponentInChildren<SLAdventureWorldDirector>(true)!=null)){errors++;report.AppendLine("- ERROR: Adventure World director is missing.");}
                        else report.AppendLine("- PASS: Adventure World director is attached (runtime resource nodes/day-night/weather generate when played).");
                        if(!sceneRoots.Any(r=>r.GetComponentInChildren<SLQuestProgression>(true)!=null)){errors++;report.AppendLine("- ERROR: Adventure World quest progression is missing.");}
                        else report.AppendLine("- PASS: Local quest progression component is attached.");
                    }
                }
                catch (Exception e) { errors++; report.AppendLine("- ERROR: Could not inspect scene `"+name+"`: "+e.Message); }
                finally { if (openedForValidation && checkScene.IsValid() && checkScene.isLoaded) EditorSceneManager.CloseScene(checkScene, true); }
            }
            if(settings.introVideo==null){warnings++;report.AppendLine("- WARNING: No intro video assigned. The opening scene will use its generated cinematic environment and title UI until a clip is assigned.");}
            else report.AppendLine("- PASS: Intro video assigned: `"+AssetDatabase.GetAssetPath(settings.introVideo)+"`.");
            warnings++; report.AppendLine("- WARNING: Procedural actors now use generated parametric body meshes, layered clothing/armor and procedural motion primitives. This is an original stylized foundation, not a guarantee of Free Fire-equivalent authored art or production Humanoid Avatar quality.");
            warnings++; report.AppendLine("- WARNING: Online multiplayer, account providers, database hosting, anti-cheat deployment and Google Play billing require separate configuration and end-to-end tests.");
            warnings++; report.AppendLine("- WARNING: Editor validation does not prove runtime gameplay, Android device compatibility, performance or multiplayer correctness. Test these in Unity and on devices.");
            report.AppendLine(); report.AppendLine("Summary: " + errors + " error(s), " + warnings + " warning(s).");
            report.AppendLine(errors==0 ? "Editor asset checks: PASS (not a production-readiness certification)." : "Editor asset checks: FAIL. Fix errors and validate again.");
            string result=report.ToString(); lastReport=result;
            if(showDialog) EditorUtility.DisplayDialog("Soulbound Legends Validation", errors==0 ? "No missing generated scene assets were detected. Review the report warnings before calling the game ready." : errors+" validation error(s) found. Review the Builder report.", "OK");
            WriteReport(result); Repaint(); return result;
        }
        void WriteReport(string report)
        {
            try { EnsureFolder("Assets/Documentation"); File.WriteAllText(ReportPath,report); AssetDatabase.ImportAsset(ReportPath); }
            catch(Exception e){Debug.LogWarning("Could not write Builder report: "+e.Message);}
        }
        void BuildAndroidApk()
        {
            string output=Path.IsPathRooted(settings.androidOutputPath)?settings.androidOutputPath:Path.Combine(Directory.GetCurrentDirectory(),settings.androidOutputPath);
            string directory=Path.GetDirectoryName(output); if(!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            var enabledScenes=EditorBuildSettings.scenes.Where(s=>s.enabled).Select(s=>s.path).ToArray();
            if(enabledScenes.Length==0){EditorUtility.DisplayDialog("Android Build","No enabled scenes were found. Run BUILD GAME first.","OK");return;}
            try
            {
                if(EditorUserBuildSettings.activeBuildTarget!=BuildTarget.Android)
                {
                    bool switched=EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android,BuildTarget.Android);
                    if(!switched){EditorUtility.DisplayDialog("Android Build","Unity could not switch to Android. Install the Android Build Support module (SDK, NDK and OpenJDK) in Unity Hub.","OK");return;}
                }
                var options=new BuildPlayerOptions{scenes=enabledScenes,locationPathName=output,target=BuildTarget.Android,options=settings.developmentBuild?BuildOptions.Development:BuildOptions.None};
                var result=BuildPipeline.BuildPlayer(options);
                lastReport += "\n\nAndroid build result: "+result.summary.result+"\nOutput: "+output+"\nErrors: "+result.summary.totalErrors+"; Warnings: "+result.summary.totalWarnings;
                WriteReport(lastReport);
                EditorUtility.DisplayDialog("Android Build",result.summary.result+"\n"+output+"\nErrors: "+result.summary.totalErrors, "OK");
            }
            catch(Exception e){lastReport="ANDROID BUILD FAILED: "+e.Message;WriteReport(lastReport);Debug.LogException(e);EditorUtility.DisplayDialog("Android Build Failed",e.Message,"OK");}
        }
        static void EnsureFolder(string path)
        {
            if(AssetDatabase.IsValidFolder(path)) return;
            string parent=Path.GetDirectoryName(path)?.Replace('\\','/'); string name=Path.GetFileName(path);
            if(!string.IsNullOrEmpty(parent)&&!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            if(!string.IsNullOrEmpty(parent)) AssetDatabase.CreateFolder(parent,name);
        }
    }
}
#endif
