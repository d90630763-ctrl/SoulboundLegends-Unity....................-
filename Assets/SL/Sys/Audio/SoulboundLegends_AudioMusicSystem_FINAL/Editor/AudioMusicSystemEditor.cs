using UnityEditor;
using UnityEngine;
using SoulboundLegends.AudioMusic;
namespace SoulboundLegends.AudioMusic.Editor
{
    [CustomEditor(typeof(SoulboundMusicManager))]
    public sealed class AudioMusicSystemEditor : UnityEditor.Editor
    {
        SerializedProperty library,entries;
        void OnEnable(){library=serializedObject.FindProperty("library");entries=serializedObject.FindProperty("musicEntries");}
        public override void OnInspectorGUI(){
            serializedObject.Update();
            EditorGUILayout.LabelField("MUSIC",EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(library);
            EditorGUILayout.PropertyField(entries,true);
            if(GUILayout.Button("+ Add Music")){int i=entries.arraySize;entries.InsertArrayElementAtIndex(i);var e=entries.GetArrayElementAtIndex(i);e.FindPropertyRelative("Name").stringValue="New Music";e.FindPropertyRelative("Context").enumValueIndex=0;}
            EditorGUILayout.Space(8);EditorGUILayout.HelpBox("Music entries are manually assigned AudioClips. Generated game audio uses the separate AudioGenerationService/Voice capability interface.",MessageType.Info);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
