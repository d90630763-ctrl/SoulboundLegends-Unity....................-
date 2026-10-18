#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.AudioWeatherWorldStreaming.Editor
{
    [CustomEditor(typeof(AudioWeatherWorldStreamingConfig))]
    public sealed class AudioWeatherWorldStreamingConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("music, SFX, ambience, weather, rain, wind, storms, day/night and scene streaming", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (AudioWeatherWorldStreamingConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
