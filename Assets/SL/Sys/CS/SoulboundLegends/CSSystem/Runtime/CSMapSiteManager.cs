using UnityEngine;

namespace SoulboundLegends.CS
{
    public class CSMapSiteManager : MonoBehaviour
    {
        public bool generateFromParentProperties = true;
        public bool allowManualOverride = true;
        public int sitesPerMap = 7;

        public CSCombatSiteDefinition SelectSite(CSMapDefinition map, int siteIndex)
        {
            if (map == null || map.sites == null || map.sites.Count == 0) return null;
            return map.sites[Mathf.Abs(siteIndex) % map.sites.Count];
        }

        public void GenerateSiteIdentity(CSMapDefinition map)
        {
            if (map == null) return;
            map.EnsureSites();
            foreach (var site in map.sites)
            {
                site.generatedIdentity = map.mapType + " combat site derived from parent map '" + map.mapName + "'. Properties: " + map.mapProperties;
                if (map.mapType == CSMapType.Ocean)
                    site.underwaterCapable = true;
                if (map.mapType == CSMapType.Air)
                    site.floatingPlatform = true;
            }
        }
    }
}
