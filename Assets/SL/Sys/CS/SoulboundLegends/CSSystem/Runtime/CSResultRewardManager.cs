using UnityEngine;

namespace SoulboundLegends.CS
{
    public class CSResultRewardManager : MonoBehaviour
    {
        public bool sendRewardsThroughServer = true;
        public bool preserveAdventureWorldEquipment = true;
        public bool preserveAdventureWorldPower = true;

        public void SubmitResult(CSResult result, int teamScoreA, int teamScoreB)
        {
            // Integration hook. The server should calculate and authorize rewards.
            // CS never destroys or transfers ownership of Adventure World equipment.
        }
    }
}
