using System;
using UnityEngine;
using UnityEngine.Events;

namespace SoulboundLegends.Level10Breakthrough
{
    [Serializable]
    public class BreakthroughResult
    {
        public bool accepted;
        public float oldLevel;
        public float newLevel;
        public float levelsGranted;
        public string reason;
    }

    [CreateAssetMenu(fileName = "Level10BreakthroughConfig", menuName = "Soulbound Legends/Progression/Level 10 Breakthrough Config")]
    public class Level10BreakthroughConfig : ScriptableObject
    {
        [Min(1)] public int unlockLevel = 1;
        [Min(1)] public int maximumLevel = 10;
        [Min(0.01f)] public float levelStep = 0.5f;
        public bool stopAtMaximumLevel = true;
        public bool requireServerAuthorization = true;
        public bool hideWhenUnavailable = true;
    }

    public class Level10BreakthroughSystem : MonoBehaviour
    {
        [SerializeField] private Level10BreakthroughConfig config;
        [SerializeField] private UnityEvent<float> onLevelChanged;
        [SerializeField] private UnityEvent onReachedLevel10;
        [SerializeField] private UnityEvent onFeatureUnavailable;

        public Level10BreakthroughConfig Config => config;

        // The server should be the authority in the live game.
        public BreakthroughResult TryApplyBreakthrough(float currentLevel, bool serverAuthorized, int availableCharges = 1)
        {
            if (config == null)
                return Reject(currentLevel, "Missing configuration.");

            if (availableCharges <= 0)
                return Reject(currentLevel, "No breakthrough charge is available.");

            if (currentLevel >= config.maximumLevel)
            {
                onFeatureUnavailable?.Invoke();
                return Reject(currentLevel, "Breakthrough is disabled at or above Level 10.");
            }

            if (config.requireServerAuthorization && !serverAuthorized)
                return Reject(currentLevel, "Server authorization required.");

            float target = Mathf.Min(
                currentLevel + config.levelStep,
                config.maximumLevel
            );

            float granted = target - currentLevel;
            if (granted <= 0f)
                return Reject(currentLevel, "No progression can be granted.");

            onLevelChanged?.Invoke(target);

            if (Mathf.Approximately(target, config.maximumLevel))
                onReachedLevel10?.Invoke();

            return new BreakthroughResult
            {
                accepted = true,
                oldLevel = currentLevel,
                newLevel = target,
                levelsGranted = granted,
                reason = "Breakthrough accepted."
            };
        }

        private BreakthroughResult Reject(float currentLevel, string reason)
        {
            return new BreakthroughResult
            {
                accepted = false,
                oldLevel = currentLevel,
                newLevel = currentLevel,
                levelsGranted = 0f,
                reason = reason
            };
        }
    }
}
