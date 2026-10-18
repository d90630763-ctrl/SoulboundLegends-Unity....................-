#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SoulboundLegends.RiftSystem.Editor
{
    public sealed class RiftSystemValidatorWindow : EditorWindow
    {
        [MenuItem("Soulbound Legends/Rift/Validate Setup")]
        private static void Open() => GetWindow<RiftSystemValidatorWindow>("Rift Validator");

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Soulbound Legends Rift System", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Select a GameObject containing RiftSystem to validate its required local references. " +
                "Production server providers must be supplied by the Combined project.",
                MessageType.Info);

            if (GUILayout.Button("Validate Selected Rift System"))
                Validate();
        }

        private static void Validate()
        {
            var system = Selection.activeGameObject?.GetComponent<RiftSystem>();
            if (system == null)
            {
                Debug.LogError("Rift validation: select a GameObject with RiftSystem.");
                return;
            }

            var hub = system.GetComponent<RiftSystemConnectionHub>();
            if (hub == null)
            {
                Debug.LogError("Rift validation: RiftSystemConnectionHub is missing.");
                return;
            }

            Debug.Log("Rift validation: local object references are present. Use the README integration checklist to verify connected production providers.");
        }
    }
}
#endif
