using UnityEditor;
using UnityEngine;
using SoulboundLegends.AudioMusic;
namespace SoulboundLegends.AudioMusic.Editor
{
    [CustomEditor(typeof(SoulboundAudioSystem))]
    public sealed class SoulboundAudioSystemEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI(){
            DrawDefaultInspector();
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("AUDIO MODULES",EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Use AudioProfile assets for SFX, AudioEventRouter for structured events, FootstepEmitter for movement, and AudioGenerationService for the optional Voice System capability.",MessageType.Info);
        }
    }
}
