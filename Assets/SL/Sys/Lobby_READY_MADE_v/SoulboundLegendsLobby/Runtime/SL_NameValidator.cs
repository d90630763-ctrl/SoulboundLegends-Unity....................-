using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SoulboundLegends.Lobby
{
    public class SL_NameValidator : MonoBehaviour
    {
        public int minimumNameLength = 3;
        public int maximumNameLength = 16;
        public string allowedCharactersRegex = "^[A-Za-z0-9_ ]+$";
        public List<string> blockedCharacters = new List<string>();
        public List<string> reservedNames = new List<string> { "Admin", "Administrator", "System", "Moderator", "GM" };
        public List<string> blockedWords = new List<string>();
        public bool trimWhitespace = true;

        public bool ValidateLocal(string value, out string message)
        {
            value = value ?? string.Empty;
            if (trimWhitespace) value = value.Trim();
            if (value.Length < minimumNameLength) { message = "Name is too short."; return false; }
            if (value.Length > maximumNameLength) { message = "Name is too long."; return false; }
            if (!Regex.IsMatch(value, allowedCharactersRegex)) { message = "Name contains invalid characters."; return false; }
            foreach (var c in blockedCharacters) if (!string.IsNullOrEmpty(c) && value.Contains(c)) { message = "Name contains a blocked character."; return false; }
            foreach (var r in reservedNames) if (!string.IsNullOrEmpty(r) && string.Equals(value, r, System.StringComparison.OrdinalIgnoreCase)) { message = "That name is reserved."; return false; }
            foreach (var w in blockedWords) if (!string.IsNullOrEmpty(w) && value.ToLowerInvariant().Contains(w.ToLowerInvariant())) { message = "Name is not allowed."; return false; }
            message = "Valid format.";
            return true;
        }
    }
}
