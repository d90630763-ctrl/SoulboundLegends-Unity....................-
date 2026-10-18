using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.CSGenerator
{
    public class CSGeneratedWorldData : MonoBehaviour
    {
        public string worldName;
        public int playersPerTeam;
        public int roundsToWin;
        public int maximumRounds;
        public float roundDurationSeconds;

        public bool eliminationOnly;
        public bool captureObjectives;
        public bool siegeObjectives;
        public bool midRoundRespawn;
        public bool adventureWorldEquipment;
        public bool adventureWorldPower;
        public bool permanentEquipmentLoss;

        public CSMapBuildData[] maps;
        public List<string> connectedSystems = new List<string>();
        public List<string> generationLog = new List<string>();

        public void Initialize(CSGenerationRequest request)
        {
            worldName = request.worldName;
        }

        public void AddConnectedSystem(CSGeneratorSystemSlot slot)
        {
            if (slot != null && !string.IsNullOrEmpty(slot.displayName))
                connectedSystems.Add(slot.displayName);
        }

        public void AddLog(string message)
        {
            generationLog.Add(message);
        }
    }

    public class CSGeneratedSiteMarker : MonoBehaviour
    {
        public CSGeneratedSiteData data;
    }
}
