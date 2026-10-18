using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.CS
{
    public class CSSpawnManager : MonoBehaviour
    {
        public Transform teamASpawnRoot;
        public Transform teamBSpawnRoot;
        public float spawnProtectionSeconds = 3f;

        public Transform GetSpawn(CSTeam team, int slotIndex)
        {
            var root = team == CSTeam.TeamA ? teamASpawnRoot : teamBSpawnRoot;
            if (root == null) return null;
            if (root.childCount == 0) return root;
            return root.GetChild(Mathf.Abs(slotIndex) % root.childCount);
        }

        public void SpawnRoundPlayers(IEnumerable<CSPlayerSlot> players)
        {
            // Integration hook: the Character/Controller system should instantiate/teleport
            // authoritative player characters at the returned spawn transforms.
        }
    }
}
