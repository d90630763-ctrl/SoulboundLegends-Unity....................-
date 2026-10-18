#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SoulboundLegends.Authentication.Editor
{
    [CustomEditor(typeof(AuthenticationSystemConnections))]
    public sealed class AuthenticationSettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(
                "Security boundary: Google secrets, JWT private keys and database credentials belong on the backend. Never store them in Unity assets.",
                MessageType.Warning);
            DrawDefaultInspector();
        }
    }
}
#endif
