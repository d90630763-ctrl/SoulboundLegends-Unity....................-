using UnityEngine;

namespace SoulboundLegends.CS
{
    public class CSBoundaryManager : MonoBehaviour
    {
        public Transform boundaryCenter;
        public float radius = 150f;
        public bool enforceBoundary = true;
        public float warningDistance = 15f;

        public bool IsInside(Vector3 position)
        {
            if (!enforceBoundary || boundaryCenter == null) return true;
            Vector3 delta = position - boundaryCenter.position;
            delta.y = 0;
            return delta.magnitude <= radius;
        }
    }
}
