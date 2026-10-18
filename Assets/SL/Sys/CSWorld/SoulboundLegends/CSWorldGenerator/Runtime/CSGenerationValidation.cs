using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.CSGenerator
{
    public static class CSGenerationValidation
    {
        public static bool Validate(CSWorldGenerator generator, out List<string> errors)
        {
            errors = new List<string>();
            if (generator == null) { errors.Add("Generator is null."); return false; }
            if (generator.request == null) errors.Add("Request is null.");
            if (generator.systems == null || generator.systems.Count == 0) errors.Add("No system slots configured.");

            if (generator.systems != null)
                foreach (var slot in generator.systems)
                    if (slot != null && slot.required && slot.systemReference == null)
                        errors.Add("Missing required connection: " + slot.displayName);

            return errors.Count == 0;
        }
    }
}
