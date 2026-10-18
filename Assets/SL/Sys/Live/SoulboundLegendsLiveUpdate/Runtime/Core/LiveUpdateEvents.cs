using System;
using UnityEngine;
namespace SoulboundLegends.LiveUpdate
{
    public static class LiveUpdateEvents
    {
        public static event Action<LiveUpdateManifest> UpdateDiscovered;
        public static event Action<float> DownloadProgress;
        public static event Action<LiveUpdateManifest> UpdateValidated;
        public static event Action<LiveUpdateManifest> UpdateApplied;
        public static event Action<string> UpdateFailed;
        public static event Action<LiveUpdateManifest> UpdateRolledBack;
        public static void Discovered(LiveUpdateManifest x)=>UpdateDiscovered?.Invoke(x);
        public static void Progress(float x)=>DownloadProgress?.Invoke(Mathf.Clamp01(x));
        public static void Validated(LiveUpdateManifest x)=>UpdateValidated?.Invoke(x);
        public static void Applied(LiveUpdateManifest x)=>UpdateApplied?.Invoke(x);
        public static void Failed(string x)=>UpdateFailed?.Invoke(x);
        public static void RolledBack(LiveUpdateManifest x)=>UpdateRolledBack?.Invoke(x);
    }
}
