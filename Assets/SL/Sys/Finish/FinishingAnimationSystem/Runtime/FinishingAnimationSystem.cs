using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SoulboundLegends.Finishing
{
    [Serializable]
    public class FinisherRequest
    {
        public FinishingAnimationDefinition definition;
        public GameObject target;
        public bool requestedBySkill;
    }

    [Serializable]
    public class FinisherValidation
    {
        public bool valid;
        public string reason;
    }

    public class FinishingAnimationSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private FinishingAnimationOwnership ownership;

        [Header("Detection")]
        [SerializeField] private LayerMask targetLayers = ~0;
        [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;
        [SerializeField] private int maxDetectedTargets = 32;

        [Header("Server")]
        [SerializeField] private bool requireServerAuthorization = true;

        [Header("Events")]
        [SerializeField] private UnityEvent<FinishingAnimationDefinition, GameObject> onFinisherStarted;
        [SerializeField] private UnityEvent<FinishingAnimationDefinition, GameObject> onFinisherCompleted;
        [SerializeField] private UnityEvent<string> onFinisherRejected;

        private readonly Dictionary<string, float> cooldownUntil = new Dictionary<string, float>();

        public List<GameObject> DetectEligibleTargets(
            Vector3 origin,
            FinishingAnimationDefinition definition,
            GameObject requester)
        {
            var result = new List<GameObject>();
            if (definition == null) return result;

            Collider[] hits = Physics.OverlapSphere(
                origin,
                definition.range,
                targetLayers,
                triggerInteraction);

            for (int i = 0; i < hits.Length && result.Count < maxDetectedTargets; i++)
            {
                GameObject target = hits[i].attachedRigidbody != null
                    ? hits[i].attachedRigidbody.gameObject
                    : hits[i].gameObject;

                if (target == null || target == requester) continue;
                if (result.Contains(target)) continue;

                result.Add(target);
            }

            return result;
        }

        public FinisherValidation ValidateRequest(
            GameObject requester,
            GameObject target,
            FinishingAnimationDefinition definition,
            bool serverAuthorized,
            bool targetIsEnemy,
            float targetHealthPercent,
            float distance,
            bool lineOfSight,
            bool requestedBySkill)
        {
            if (requester == null || target == null || definition == null)
                return Reject("Missing finisher request data.");

            if (requireServerAuthorization && !serverAuthorized)
                return Reject("Server authorization required.");

            if (definition.requiresOwnership &&
                (ownership == null || !ownership.Owns(definition)))
                return Reject("Finisher is not owned.");

            if (!definition.usableAsSkill && requestedBySkill)
                return Reject("This finisher cannot be triggered by a skill.");

            if (targetHealthPercent > definition.maximumTargetHealthPercent)
                return Reject("Target is above the finisher health threshold.");

            if (distance > definition.range)
                return Reject("Target is outside finisher range.");

            if (definition.requireLineOfSight && !lineOfSight)
                return Reject("No line of sight.");

            if (definition.targetRule == FinisherTargetRule.EnemyOnly && !targetIsEnemy)
                return Reject("Target is not an eligible enemy.");

            if (IsOnCooldown(definition))
                return Reject("Finisher is on cooldown.");

            return new FinisherValidation { valid = true, reason = "Finisher request valid." };
        }

        public bool IsOnCooldown(FinishingAnimationDefinition definition)
        {
            if (definition == null || string.IsNullOrWhiteSpace(definition.finisherId))
                return true;

            return cooldownUntil.TryGetValue(definition.finisherId, out float until) &&
                   Time.time < until;
        }

        public void MarkUsed(FinishingAnimationDefinition definition)
        {
            if (definition == null || string.IsNullOrWhiteSpace(definition.finisherId))
                return;

            cooldownUntil[definition.finisherId] =
                Time.time + definition.cooldownSeconds;
        }

        // Call only after the authoritative server approves the request.
        public void BeginAuthorizedFinisher(
            GameObject requester,
            GameObject target,
            FinishingAnimationDefinition definition)
        {
            if (requester == null || target == null || definition == null)
                return;

            MarkUsed(definition);
            onFinisherStarted?.Invoke(definition, target);
        }

        // The final combat/death system should perform the authoritative death.
        public void CompleteAuthorizedFinisher(
            GameObject target,
            FinishingAnimationDefinition definition)
        {
            if (target == null || definition == null)
                return;

            onFinisherCompleted?.Invoke(definition, target);
        }

        private FinisherValidation Reject(string reason)
        {
            onFinisherRejected?.Invoke(reason);
            return new FinisherValidation { valid = false, reason = reason };
        }

        private void OnDrawGizmosSelected()
        {
            // Visualization is intentionally handled by the final integration prefab.
        }
    }
}
