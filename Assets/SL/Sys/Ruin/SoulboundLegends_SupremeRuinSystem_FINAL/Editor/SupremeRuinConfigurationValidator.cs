#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace SoulboundLegends.SupremeRuin.Editor
{
    public static class SupremeRuinConfigurationValidator
    {
        [MenuItem("Soulbound Legends/Supreme Ruin/Validate Selected Configuration")]
        public static void Validate()
        {
            var config = Selection.activeObject as SupremeRuinConfiguration;
            if (config == null)
            {
                Debug.LogError("Select a SupremeRuinConfiguration asset first.");
                return;
            }

            int errors = 0;
            var godIds = new HashSet<string>();
            foreach (var god in config.Gods)
            {
                if (god == null || string.IsNullOrWhiteSpace(god.Id) || string.IsNullOrWhiteSpace(god.InheritanceId))
                {
                    Debug.LogError("God definition has missing ID or inheritance ID.", config);
                    errors++;
                }
                else if (!godIds.Add(god.Id))
                {
                    Debug.LogError($"Duplicate God ID: {god.Id}", config);
                    errors++;
                }
            }

            var ruinIds = new HashSet<string>();
            foreach (var ruin in config.Ruins)
            {
                if (ruin == null || string.IsNullOrWhiteSpace(ruin.Id))
                {
                    Debug.LogError("Ruin definition has missing ID.", config);
                    errors++;
                    continue;
                }
                if (!ruinIds.Add(ruin.Id)) { Debug.LogError($"Duplicate Ruin ID: {ruin.Id}", config); errors++; }
                if (ruin.RequiredCreatures == null || ruin.RequiredCreatures.Count != 5)
                {
                    Debug.LogError($"Ruin {ruin.Id} must contain exactly five creature slots.", config);
                    errors++;
                }
                if (string.IsNullOrWhiteSpace(ruin.GodId) || config.FindGod(ruin.GodId) == null)
                {
                    Debug.LogError($"Ruin {ruin.Id} references a missing God.", config);
                    errors++;
                }
                if (ruin.MaxPlayers != 1)
                    Debug.LogWarning($"Ruin {ruin.Id}: one-player challenge is the established rule; MaxPlayers is {ruin.MaxPlayers}.", config);
            }

            foreach (var creature in config.DivineCreatures)
            {
                if (creature == null || string.IsNullOrWhiteSpace(creature.Id))
                {
                    Debug.LogError("Divine creature has missing ID.", config);
                    errors++;
                }
            }

            Debug.Log(errors == 0
                ? "Supreme Ruin configuration validation passed."
                : $"Supreme Ruin configuration validation found {errors} error(s).", config);
        }
    }
}
#endif
