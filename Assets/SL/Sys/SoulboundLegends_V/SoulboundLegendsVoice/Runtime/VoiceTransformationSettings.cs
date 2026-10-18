using System;
using UnityEngine;

namespace SoulboundLegends.Voice
{
    [Serializable]
    public class VoiceTransformationSettings
    {
        [Range(0f, 1f)] public float gender = 0.5f;
        [Range(0.5f, 2f)] public float pitch = 1f;
        [Range(0f, 1f)] public float deepness = 0.5f;
        [Range(0f, 1f)] public float brightness = 0.5f;
        [Range(0f, 1f)] public float volume = 1f;

        public VoiceTransformationSettings Clone()
        {
            return new VoiceTransformationSettings
            {
                gender = gender,
                pitch = pitch,
                deepness = deepness,
                brightness = brightness,
                volume = volume
            };
        }
    }
}
