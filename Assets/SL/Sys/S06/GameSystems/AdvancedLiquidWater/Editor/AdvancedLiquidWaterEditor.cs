#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.AdvancedLiquidWater.Editor
{
    [CustomEditor(typeof(AdvancedLiquidWaterConfig))]
    public sealed class AdvancedLiquidWaterConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("water/river/lake/sea/ocean/swamp/lava/poison/custom liquids, bodies, custom shapes, channel linking, flow, pooling, buoyancy, currents, waves", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (AdvancedLiquidWaterConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
