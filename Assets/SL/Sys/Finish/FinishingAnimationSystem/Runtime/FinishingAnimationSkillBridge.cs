using UnityEngine;

namespace SoulboundLegends.Finishing
{
    public class FinishingAnimationSkillBridge : MonoBehaviour
    {
        [SerializeField] private FinishingAnimationSystem finishingSystem;

        // A Skill System can call this after its own server-side skill validation.
        public bool RequestSkillFinisher(
            GameObject caster,
            GameObject target,
            FinishingAnimationDefinition definition,
            bool serverAuthorized,
            bool targetIsEnemy,
            float targetHealthPercent,
            float distance,
            bool lineOfSight)
        {
            if (finishingSystem == null || definition == null || !definition.skillCanTriggerFinisher)
                return false;

            FinisherValidation validation = finishingSystem.ValidateRequest(
                caster,
                target,
                definition,
                serverAuthorized,
                targetIsEnemy,
                targetHealthPercent,
                distance,
                lineOfSight,
                true);

            if (!validation.valid)
                return false;

            finishingSystem.BeginAuthorizedFinisher(caster, target, definition);
            return true;
        }
    }
}
