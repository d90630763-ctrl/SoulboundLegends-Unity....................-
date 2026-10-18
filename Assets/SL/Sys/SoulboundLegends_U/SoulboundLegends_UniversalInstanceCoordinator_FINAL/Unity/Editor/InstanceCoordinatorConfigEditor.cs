#if UNITY_EDITOR
using UnityEditor; using UnityEngine;
namespace SoulboundLegends.InstanceCoordinator.Editor
{
 [CustomEditor(typeof(InstanceCoordinatorConfig))] public sealed class InstanceCoordinatorConfigEditor:UnityEditor.Editor
 {
  public override void OnInspectorGUI(){serializedObject.Update();DrawDefaultInspector();EditorGUILayout.Space();EditorGUILayout.HelpBox("Capacities, timeouts and type profiles are configuration. Server authority remains authoritative; client values do not override server limits.",MessageType.Info);if(GUILayout.Button("Validate Configuration")){var c=(InstanceCoordinatorConfig)target;if(c.DefaultMaximumCapacity<1||c.MaximumInstances<1)EditorUtility.DisplayDialog("Invalid configuration","Maximum capacity and maximum instances must be at least 1.","OK");else EditorUtility.DisplayDialog("Valid","Coordinator configuration passed local validation.","OK");}serializedObject.ApplyModifiedProperties();}
 }
}
#endif
