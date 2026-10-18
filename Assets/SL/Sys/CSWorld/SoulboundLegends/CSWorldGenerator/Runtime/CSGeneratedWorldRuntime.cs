using UnityEngine;

namespace SoulboundLegends.CSGenerator
{
    public class CSGeneratedWorldRuntime : MonoBehaviour
    {
        public CSGeneratedWorldData worldData;
        public bool readyForLobbyEntry;
        public bool readyForServerInstance;

        public void Initialize()
        {
            readyForLobbyEntry = worldData != null && worldData.maps != null && worldData.maps.Length == 5;
            readyForServerInstance = readyForLobbyEntry && worldData.eliminationOnly;
        }
    }
}
