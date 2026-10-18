#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoulboundLegends.Lobby.Editor
{
    [CustomEditor(typeof(SL_LobbySystem))]
    public class SL_LobbyEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space(12);
            EditorGUILayout.HelpBox("Soulbound Legends Lobby: the package supplies a procedural 3D fantasy lobby baseline, default UI graphics, entry flow, mode locking, name validation, settings, emotes, integration hooks and server-authority hooks. Replace/extend assets through the lists above.", MessageType.Info);
            var lobby = (SL_LobbySystem)target;
            if (GUILayout.Button("GENERATE / REFRESH 3D LOBBY"))
            {
                if (lobby.environmentGenerator == null) lobby.environmentGenerator = lobby.GetComponent<SL_EnvironmentGenerator>() ?? lobby.gameObject.AddComponent<SL_EnvironmentGenerator>();
                lobby.environmentGenerator.referenceImage = lobby.referenceImage;
                if (lobby.environmentProfiles.Count == 0) lobby.environmentProfiles.Add(new SL_EnvironmentProfile());
                lobby.environmentGenerator.profile = lobby.environmentProfiles[Mathf.Clamp(lobby.activeEnvironmentProfile, 0, lobby.environmentProfiles.Count - 1)];
                lobby.environmentGenerator.Generate();
                EditorUtility.SetDirty(lobby);
            }
            if (GUILayout.Button("RUN COMPATIBILITY CHECK"))
            {
                if (lobby.compatibilityChecker == null) lobby.compatibilityChecker = lobby.gameObject.AddComponent<SL_CompatibilityChecker>();
                lobby.compatibilityChecker.lobby = lobby;
                lobby.compatibilityChecker.integration = lobby.integrationManager;
                lobby.compatibilityChecker.server = lobby.serverBridge;
                lobby.compatibilityChecker.RunCheck();
            }
            if (GUILayout.Button("CREATE READY-MADE LOBBY SCENE")) CreateDemoScene();
            if (GUILayout.Button("BUILD READY-MADE UI NOW"))
            {
                var ready = lobby.GetComponent<SL_ReadyMadeUI>() ?? lobby.gameObject.AddComponent<SL_ReadyMadeUI>();
                ready.Build();
                EditorUtility.SetDirty(lobby);
            }
        }

        [MenuItem("Tools/Soulbound Legends/Create Demo Lobby Scene")]
        public static void CreateDemoScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var go = new GameObject("Soulbound Legends Lobby System");
            var lobby = go.AddComponent<SL_LobbySystem>();
            go.AddComponent<SL_NameValidator>();
            go.AddComponent<SL_SystemIntegrationManager>();
            go.AddComponent<SL_SceneLoader>();
            go.AddComponent<SL_ReadyMadeUI>();
            var env = go.AddComponent<SL_EnvironmentGenerator>();
            lobby.environmentGenerator = env;
            env.lobbyCamera = CreateCamera(go.transform);
            lobby.lobbyCamera = env.lobbyCamera;
            lobby.referenceImage = Resources.Load<Texture2D>("LobbyReference");
            lobby.autoStart = true;
            lobby.autoBuildUI = true;
            string path = "Assets/SoulboundLegendsLobby/Sample/Scenes/SoulboundLegendsLobbyDemo.unity";
            EditorSceneManager.SaveScene(scene, path);
            Selection.activeGameObject = go;
            EditorGUIUtility.PingObject(go);
            Debug.Log("Soulbound Legends: Demo Lobby Scene created at " + path);
        }

        private static Camera CreateCamera(Transform parent)
        {
            var go = new GameObject("Lobby Camera");
            go.transform.SetParent(parent, false);
            var cam = go.AddComponent<Camera>();
            cam.fieldOfView = 55f;
            go.transform.position = new Vector3(0, 4.5f, -11f);
            go.transform.rotation = Quaternion.Euler(12f, 0f, 0f);
            return cam;
        }
    }
}
#endif
