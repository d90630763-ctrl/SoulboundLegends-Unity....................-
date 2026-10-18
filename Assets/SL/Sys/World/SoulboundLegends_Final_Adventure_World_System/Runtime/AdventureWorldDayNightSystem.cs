using UnityEngine;

namespace SoulboundLegends.AdventureWorld
{
    public class AdventureWorldDayNightSystem : MonoBehaviour
    {
        [Range(0,24)] public float timeOfDay = 8f;
        public float fullDaySeconds = 1200f;
        public Light sun;
        public AnimationCurve sunIntensity = AnimationCurve.Linear(0,0,1,1);
        public void Initialize() { if (sun == null) sun = RenderSettings.sun; }
        void Update()
        {
            if (fullDaySeconds <= 0) return;
            timeOfDay = (timeOfDay + Time.deltaTime * 24f / fullDaySeconds) % 24f;
            float normalized = timeOfDay / 24f;
            if (sun != null) sun.transform.rotation = Quaternion.Euler(normalized * 360f - 90f, 170f, 0f);
            if (sun != null) sun.intensity = Mathf.Lerp(0.05f, 1f, Mathf.Clamp01(Mathf.Sin(normalized * Mathf.PI * 2f) * 0.5f + 0.5f));
        }
    }
}
