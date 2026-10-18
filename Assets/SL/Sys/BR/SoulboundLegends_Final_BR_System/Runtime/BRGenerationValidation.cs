using UnityEngine;

namespace SoulboundLegends.BR
{
    public static class BRGenerationValidation
    {
        public static bool IsAdventureWorldSourceRuleValid(BRGenerationRequest request)
        {
            return request != null &&
                   request.useAdventureWorldEquipment &&
                   request.useAdventureWorldSkills &&
                   request.useAdventureWorldPowers &&
                   request.useAdventureWorldStats &&
                   !request.permanentAdventureWorldLossOnDeath;
        }
    }
}
