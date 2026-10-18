using System;
using UnityEngine;
namespace SoulboundLegends.AudioMusic
{
    [Serializable]
    public struct AudioRequest
    {
        public AudioCategory Category;
        public SoundType SoundType;
        public string Context;
        public string SourceId;
        public SurfaceType Surface;
        public ElementType Element;
        public WeaponType Weapon;
        public string CreatureType;
        public string SkillType;
        public string Environment;
        [Range(0,1)] public float Intensity;
        [Min(0)] public float Duration;
        public Vector3 Position;
        public bool HasPosition;
        [Range(0,256)] public int Priority;
        [Range(0.1f,3f)] public float VolumeMultiplier;
        [Range(0.5f,2f)] public float PitchMultiplier;
        public bool Loop;
        public static AudioRequest Create(SoundType type, AudioCategory category = AudioCategory.SFX, Vector3 position = default, int priority = 128)
        {
            return new AudioRequest { Category=category, SoundType=type, Position=position, HasPosition=true, Priority=priority,
                VolumeMultiplier=1f, PitchMultiplier=1f, Intensity=1f };
        }
    }
}
