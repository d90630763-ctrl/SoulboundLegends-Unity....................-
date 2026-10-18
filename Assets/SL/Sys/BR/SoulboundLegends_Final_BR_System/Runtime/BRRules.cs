using UnityEngine;

namespace SoulboundLegends.BR
{
    [CreateAssetMenu(menuName = "Soulbound Legends/BR/Rules", fileName = "BRRules")]
    public sealed class BRRules : ScriptableObject
    {
        [Header("Core")]
        public string purpose = "Adapt & Survive";
        public bool lastPlayerOrTeamWins = true;

        [Header("Adventure World Source")]
        public bool weaponsComeFromAdventureWorld = true;
        public bool skillsComeFromAdventureWorld = true;
        public bool powersComeFromAdventureWorld = true;
        public bool equipmentComesFromAdventureWorld = true;
        public bool permanentLossOnDeath = false;

        [Header("Entry")]
        public bool randomAnimeTeleport = true;
        public bool visibleArrivalBeam = true;

        [Header("Zone")]
        public bool shrinkingSafeZone = true;
        public int phases = 6;
        public float outsideDamagePerSecond = 10f;

        [Header("Team Play")]
        public bool solo = true;
        public bool duo = true;
        public bool squad = true;
        public bool downedState = true;
        public bool revival = true;
        public bool spectating = true;

        [Header("World")]
        public bool worldEvents = true;
        public bool monsters = true;
        public bool npcWorldContent = true;
        public bool destructibleEnvironment = true;
    }
}
