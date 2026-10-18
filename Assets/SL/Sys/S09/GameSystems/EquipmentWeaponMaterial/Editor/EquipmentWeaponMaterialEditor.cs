#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.EquipmentWeaponMaterial.Editor
{
    [CustomEditor(typeof(EquipmentWeaponMaterialConfig))]
    public sealed class EquipmentWeaponMaterialConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("equipment slots, weapons, armor, materials H/S/T/B/M, runes, effects, damage, storage, aetherium blade and stat caps", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (EquipmentWeaponMaterialConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
