using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.LiveUpdate
{
    public enum UpdateContentType { Configuration, Text, Localization, Audio, Music, VFX, UI, Item, Skill, Character, Monster, NPC, Quest, Event, Reward, World, Map, Spawn, Economy, GameMode, Cosmetic, AddressableCatalog, ClientPatch }
    public enum UpdateReleaseStage { Draft, Internal, Canary, Staged, Live, Paused, RolledBack, Retired }
    public enum UpdateApplyMode { HotApply, RestartRequired, ClientUpdateRequired }

    [Serializable] public class UpdateDependency { public string id; public string minimumVersion; }
    [Serializable] public class UpdateFile { public string id; public string relativePath; public string url; public string sha256; public long sizeBytes; public bool required; }

    [Serializable]
    public class LiveUpdateManifest
    {
        public string manifestId;
        public string gameId = "SoulboundLegends";
        public string channel = "production";
        public int schemaVersion = 1;
        public string contentVersion = "1.0.0";
        public string minimumClientVersion = "1.0.0";
        public string maximumClientVersion = "";
        public string serverApiVersion = "1";
        public UpdateReleaseStage stage = UpdateReleaseStage.Draft;
        public UpdateApplyMode applyMode = UpdateApplyMode.HotApply;
        public bool mandatory;
        public bool rollbackAllowed = true;
        public bool maintenanceRequired;
        [Range(0,100)] public int rolloutPercent = 100;
        public long publishedUnixSeconds;
        public List<string> tags = new List<string>();
        public List<UpdateDependency> dependencies = new List<UpdateDependency>();
        public List<UpdateFile> files = new List<UpdateFile>();
        public string configJson = "{}";
    }

    [Serializable] public class UpdateManifestEnvelope { public LiveUpdateManifest manifest; public string signature; public string keyId; }
    [Serializable] public class InstalledUpdateState { public string contentVersion = "0.0.0"; public string manifestId; public string configJson = "{}"; public List<string> installedFiles = new List<string>(); }
    [Serializable] public class UpdateResult { public bool success; public bool restartRequired; public bool clientUpdateRequired; public string message; public string version; }
    [Serializable] public class UpdateCheckResult { public bool updateAvailable; public bool compatible; public bool mandatory; public LiveUpdateManifest manifest; public string reason; }

    public static class UpdateJson
    {
        public static string ToJson<T>(T value) => JsonUtility.ToJson(value, true);
        public static T FromJson<T>(string json) => JsonUtility.FromJson<T>(json);
    }
}
