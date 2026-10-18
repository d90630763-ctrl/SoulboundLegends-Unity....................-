using UnityEngine;
using UnityEngine.Video;

namespace SoulboundLegends.Runtime
{
    [CreateAssetMenu(menuName = "Soulbound Legends/Builder Settings", fileName = "SLBuilderSettings")]
    public sealed class SLBuilderSettings : ScriptableObject
    {
        [Header("Opening Cinematic")]
        public VideoClip introVideo;
        public bool allowSkipIntro = true;
        public bool autoplayIntro = true;

        [Header("Generation")]
        public int worldSeed = 20261012;
        [Range(4, 80)] public int vegetationCount = 28;
        [Range(0, 30)] public int monsterCount = 8;
        public bool generateAllNineScenes = true;
        public bool includeProceduralActors = true;
        public bool generateUI = true;
        public bool addThirdPersonCamera = true;

        [Header("Advanced Visual Generation")]
        [Tooltip("Generate a high-resolution procedural terrain mesh with layered elevation, ridges and a flatter playable center.")]
        public bool generateComplexTerrain = true;
        [Range(33, 161)] public int terrainResolution = 97;
        [Range(0.5f, 5f)] public float terrainRelief = 2.4f;
        [Tooltip("Use denser parametric surfaces for generated character parts.")]
        public bool highDetailCharacterMeshes = true;
        [Tooltip("Apply mobile-conscious camera, shadow and frame-rate defaults to generated scenes.")]
        public bool applyPerformanceDefaults = true;

        [Header("Android")]
        public string androidOutputPath = "Builds/Android/SoulboundLegends.apk";
        public bool developmentBuild = false;
    }
}
