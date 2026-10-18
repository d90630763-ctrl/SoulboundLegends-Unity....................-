#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace SoulboundLegends.Animation.Editor
{
    public sealed class SLAnimationDirectorWindow:EditorWindow
    {
        SLAnimationDefinition definition; Vector2 scroll;
        [MenuItem("Soulbound Legends/Animation/Universal Animation Director")] public static void Open()=>GetWindow<SLAnimationDirectorWindow>("SL Animation Director");
        void OnGUI(){EditorGUILayout.LabelField("Soulbound Legends — Universal Animation Director",EditorStyles.boldLabel);definition=(SLAnimationDefinition)EditorGUILayout.ObjectField("Animation Definition",definition,typeof(SLAnimationDefinition),false);if(!definition){if(GUILayout.Button("Create Definition Asset")){var path=EditorUtility.SaveFilePanelInProject("Create Animation Definition","SL_AnimationDefinition","asset","Choose location.");if(!string.IsNullOrEmpty(path)){definition=CreateInstance<SLAnimationDefinition>();AssetDatabase.CreateAsset(definition,path);AssetDatabase.SaveAssets();}}return;}definition.description=EditorGUILayout.TextArea(definition.description,GUILayout.MinHeight(100));if(GUILayout.Button("Generate / Rebuild Action Sequence",GUILayout.Height(30)))Generate();if(GUILayout.Button("Add Runtime Animation Components To Selected Character"))AddComponents();EditorGUILayout.Space(8);EditorGUILayout.LabelField("Generated Actions",EditorStyles.boldLabel);scroll=EditorGUILayout.BeginScrollView(scroll);for(int i=0;i<definition.actions.Count;i++){var a=definition.actions[i];EditorGUILayout.BeginVertical("box");EditorGUILayout.LabelField((i+1)+". "+a.id,a.type.ToString());EditorGUILayout.LabelField(a.instruction);EditorGUILayout.LabelField("Duration: "+a.duration.ToString("0.00")+"s");EditorGUILayout.EndVertical();}EditorGUILayout.EndScrollView();}
        void Generate(){var p=new SLDescriptionMotionProvider();definition.actions=p.BuildActions(definition.description);definition.markers=new List<SLAnimationMarker>();float t=0;foreach(var a in definition.actions){a.markerOnStart=a.id+"_Start";a.markerOnEnd=a.id+"_End";definition.markers.Add(new SLAnimationMarker{id=a.markerOnStart,time=t,vfxEventId=a.vfxEventId,audioEventId=a.audioEventId,gameplayEventId=a.gameplayEventId});t+=Mathf.Max(.05f,a.duration);definition.markers.Add(new SLAnimationMarker{id=a.markerOnEnd,time=t});}EditorUtility.SetDirty(definition);AssetDatabase.SaveAssets();}
        static void AddComponents(){var go=Selection.activeGameObject;if(!go){EditorUtility.DisplayDialog("Animation Director","Select a character first.","OK");return;}if(!go.GetComponent<SLUniversalAnimationActor>())Undo.AddComponent<SLUniversalAnimationActor>(go);if(!go.GetComponent<SLAnimationIKDriver>())Undo.AddComponent<SLAnimationIKDriver>(go);if(!go.GetComponent<SLAnimationRuntime>())Undo.AddComponent<SLAnimationRuntime>(go);if(!go.GetComponent<SLAnimationSystemBridge>())Undo.AddComponent<SLAnimationSystemBridge>(go);}
    }
}
#endif
