using UnityEngine;

namespace SoulboundLegends.Voice
{
    public static class VoicePresetLibrary
    {
        public static VoiceTransformationSettings Feminine()
        {
            return new VoiceTransformationSettings
            {
                gender = 0f, pitch = 1.16f, deepness = .25f, brightness = .72f, volume = 1f
            };
        }

        public static VoiceTransformationSettings Neutral()
        {
            return new VoiceTransformationSettings
            {
                gender = .5f, pitch = 1f, deepness = .5f, brightness = .5f, volume = 1f
            };
        }

        public static VoiceTransformationSettings Masculine()
        {
            return new VoiceTransformationSettings
            {
                gender = 1f, pitch = .84f, deepness = .78f, brightness = .38f, volume = 1f
            };
        }

        public static VoiceTransformationSettings Child()
        {
            return new VoiceTransformationSettings
            {
                gender = .25f, pitch = 1.3f, deepness = .1f, brightness = .85f, volume = 1f
            };
        }

        public static VoiceTransformationSettings Elder()
        {
            return new VoiceTransformationSettings
            {
                gender = .65f, pitch = .9f, deepness = .65f, brightness = .25f, volume = .9f
            };
        }

        public static VoiceTransformationSettings Demon()
        {
            return new VoiceTransformationSettings
            {
                gender = .75f, pitch = .7f, deepness = .95f, brightness = .15f, volume = 1f
            };
        }
    }
}
