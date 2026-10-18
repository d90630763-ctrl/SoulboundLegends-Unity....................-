using System;
using UnityEngine;
namespace GameSystems.DivineInheritanceWorldState {
    public enum InheritanceType { Water, Fire, Earth, Void, Thunder, Nature, Light, Air, Ice, Omni }
    [CreateAssetMenu(fileName="InheritanceDefinition",menuName="Game Systems/Divine Inheritance/Definition")]
    public sealed class InheritanceDefinition:ScriptableObject {
        public InheritanceType type; public string name; [TextArea] public string lore;
        public int challengeLevel=1; public bool claimed;
    }
    public sealed class WorldInheritanceState:ScriptableObject {
        public bool barrierActive=true; public bool demonWarActive=false; public int requiredWorldAverageLevel=100;
        public string[] claimedPlayerIds=new string[10];
        public bool CanClaim(string playerId, InheritanceType type) {
            if(string.IsNullOrWhiteSpace(playerId)||demonWarActive) return false;
            foreach(var id in claimedPlayerIds) if(id==playerId) return false;
            return true;
        }
        public bool AllTenClaimed() {
            int count=0; foreach(var id in claimedPlayerIds) if(!string.IsNullOrWhiteSpace(id)) count++;
            return count>=10;
        }
    }
}