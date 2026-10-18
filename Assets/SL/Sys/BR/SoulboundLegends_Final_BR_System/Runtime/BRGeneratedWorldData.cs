using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.BR
{
    public sealed class BRGeneratedWorldData : MonoBehaviour
    {
        public string worldName;
        public BRGenerationRequest rules;
        public List<BRMapBuildData> maps = new List<BRMapBuildData>();
        public List<string> connectedSystems = new List<string>();
        public List<string> generationReports = new List<string>();
        public bool readyForLobby;
        public bool readyForServerInstance;
    }

    public sealed class BRGeneratedMapMarker : MonoBehaviour
    {
        public string mapId;
        public string mapName;
        public string environmentType;
    }

    public sealed class BRGeneratedPOIMarker : MonoBehaviour
    {
        public string poiId;
        public string poiName;
        public string poiType;
        public bool highValue;
    }
}
