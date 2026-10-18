#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace SoulboundLegends.EditorTools
{
    /// <summary>
    /// Keeps the Windows standalone project explicitly D3D12-ready while retaining
    /// D3D11 as a compatibility fallback. This does not force D3D12 on unsupported
    /// hardware at runtime; Unity chooses the first supported API in the configured list.
    /// </summary>
    [InitializeOnLoad]
    internal static class SLD3D12GraphicsConfigurator
    {
        private static readonly GraphicsDeviceType[] DesiredApis =
        {
            GraphicsDeviceType.Direct3D12,
            GraphicsDeviceType.Direct3D11
        };

        static SLD3D12GraphicsConfigurator()
        {
            EditorApplication.delayCall += Apply;
        }

        private static void Apply()
        {
            try
            {
                const BuildTarget target = BuildTarget.StandaloneWindows64;

                if (PlayerSettings.GetUseDefaultGraphicsAPIs(target))
                    PlayerSettings.SetUseDefaultGraphicsAPIs(target, false);

                var current = PlayerSettings.GetGraphicsAPIs(target);
                bool matches = current != null && current.Length == DesiredApis.Length;
                if (matches)
                {
                    for (int i = 0; i < current.Length; i++)
                    {
                        if (current[i] != DesiredApis[i])
                        {
                            matches = false;
                            break;
                        }
                    }
                }

                if (!matches)
                    PlayerSettings.SetGraphicsAPIs(target, DesiredApis);

                Debug.Log("[SL] Windows graphics APIs configured: Direct3D 12 (primary), Direct3D 11 (fallback). The project remains graphics-API independent.");
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[SL] Could not automatically configure Windows D3D12/D3D11 graphics APIs yet. Open Project Settings > Player > Other Settings > Rendering and configure them manually. Details: " + ex.Message);
            }
        }
    }
}
#endif
