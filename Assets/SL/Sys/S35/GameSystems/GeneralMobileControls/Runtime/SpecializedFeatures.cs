using UnityEngine;
namespace GameSystems.GeneralMobileControls {
    public enum DodgeDirection { Up, Down, Left, Right }
    public sealed class MobileControlProfile:ScriptableObject {
        public float sensitivity=1, multitouchLimit=5; public bool swipeLimbs=true, directionalDodge=true;
        public float dodgeDistance=3, dodgeCooldown=.75f;
    }
    public sealed class MobileControlRouter:MonoBehaviour {
        public Vector2 move; public bool leftHand,rightHand,leftLeg,rightLeg,defense;
        public DodgeDirection lastDodge;
        public void SetDodge(DodgeDirection d) => lastDodge=d;
    }
}