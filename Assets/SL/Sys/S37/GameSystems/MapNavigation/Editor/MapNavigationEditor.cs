#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.MapNavigation.Editor
{
    [CustomEditor(typeof(MapNavigationConfig))]
    public sealed class MapNavigationConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("world map, minimap, markers, destinations, navigation, fog-of-war and optimization", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (MapNavigationConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
