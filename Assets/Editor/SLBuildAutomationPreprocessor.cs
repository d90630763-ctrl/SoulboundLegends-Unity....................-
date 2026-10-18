#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace SoulboundLegends.EditorTools
{
    /// <summary>
    /// Ensures Unity Cloud Build and local command-line builds generate current scenes before packaging.
    /// Build Game can still be run manually in the Editor; this callback prevents stale placeholder scenes
    /// from being compiled when the repository is built directly by an automated build service.
    /// </summary>
    public sealed class SLBuildAutomationPreprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => -1000;
        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("Soulbound Legends: preparing generated scenes for " + report.summary.platform + " build.");
            SoulboundLegendsGameBuilder.BuildScenesForAutomatedBuild();
        }
    }
}
#endif
