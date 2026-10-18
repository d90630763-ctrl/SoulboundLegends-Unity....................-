using UnityEngine;

namespace SoulboundLegends.BR
{
    public sealed class BRSafeZoneSystem : MonoBehaviour
    {
        public int phaseCount = 6;
        public float initialRadius = 1f;
        public float finalRadius = 0.08f;
        public float damagePerSecondOutside = 10f;
        public int currentPhase;

        public void StartPhase(int phase)
        {
            currentPhase = Mathf.Clamp(phase, 0, phaseCount);
        }

        public float GetNormalizedRadius()
        {
            if (phaseCount <= 0) return finalRadius;
            float t = Mathf.Clamp01((float)currentPhase / phaseCount);
            return Mathf.Lerp(initialRadius, finalRadius, t);
        }
    }
}
