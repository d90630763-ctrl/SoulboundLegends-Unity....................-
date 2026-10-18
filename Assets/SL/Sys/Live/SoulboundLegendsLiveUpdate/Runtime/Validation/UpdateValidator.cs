using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
namespace SoulboundLegends.LiveUpdate
{
    public sealed class UpdateValidator
    {
        public bool ValidateManifest(LiveUpdateManifest m, string clientVersion, string gameId, out string error)
        {
            error = null;
            if (m == null) { error = "Manifest is null."; return false; }
            if (m.gameId != gameId) { error = "Manifest belongs to another game."; return false; }
            if (m.schemaVersion != 1) { error = "Unsupported manifest schema."; return false; }
            if (!VersionAtLeast(clientVersion, m.minimumClientVersion)) { error = "Client is too old."; return false; }
            if (!string.IsNullOrWhiteSpace(m.maximumClientVersion) && VersionAtLeast(clientVersion, m.maximumClientVersion) && clientVersion != m.maximumClientVersion) { error = "Client is too new for this manifest."; return false; }
            if (string.IsNullOrWhiteSpace(m.contentVersion)) { error = "Missing content version."; return false; }
            foreach (var f in m.files) if (f == null || string.IsNullOrWhiteSpace(f.id) || string.IsNullOrWhiteSpace(f.relativePath)) { error = "Invalid update file entry."; return false; }
            return true;
        }
        public static bool VerifySha256(byte[] data, string expected)
        {
            if (string.IsNullOrWhiteSpace(expected)) return false;
            using var sha = SHA256.Create(); var hash = sha.ComputeHash(data); var actual = Convert.ToHexString(hash).ToLowerInvariant(); return actual == expected.Trim().ToLowerInvariant();
        }
        public static bool VersionAtLeast(string a, string b)
        {
            if (!Version.TryParse(Normalize(a), out var av)) return false;
            if (!Version.TryParse(Normalize(b), out var bv)) return false;
            return av >= bv;
        }
        private static string Normalize(string s) => (s ?? "0.0.0").Split('+')[0].Split('-')[0];
    }
}
