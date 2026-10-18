using UnityEngine;

namespace SoulboundLegends.AdventureWorld
{
    public class AdventureWorldCityGraduation : MonoBehaviour
    {
        public int requiredLevel = 10;
        public bool requireToken = true;
        public bool HasGraduated(PlayerWorldState state, string tokenId, bool tokenValidated)
        {
            if (state == null || state.level < requiredLevel) return false;
            return !requireToken || tokenValidated;
        }
        public bool SetPermanentCityStart(PlayerWorldState state, string cityRegionId)
        {
            if (state == null || string.IsNullOrEmpty(cityRegionId)) return false;
            state.permanentCityStartRegionId = cityRegionId; return true;
        }
    }
}
