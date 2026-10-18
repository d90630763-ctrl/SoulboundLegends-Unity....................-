using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace SoulboundLegends.LiveUpdate
{
    public sealed class SoulboundLegendsLiveUpdateSystem : MonoBehaviour
    {
        public static SoulboundLegendsLiveUpdateSystem Instance { get; private set; }
        [Header("Identity")][SerializeField] private string gameId = "SoulboundLegends";
        [SerializeField] private string clientVersion = "1.0.0";
        [SerializeField] private string channel = "production";
        [Header("Remote Provider")][SerializeField] private string manifestBaseUrl = "";
        [Header("Safety")][SerializeField] private bool automaticCheckOnStart = true;
        [SerializeField] private bool applyHotUpdatesAutomatically = true;
        [SerializeField] private int requestTimeoutSeconds = 30;
        public InstalledUpdateState State { get; private set; }
        public LiveUpdateManifest AvailableManifest { get; private set; }
        private UpdateStorage storage; private UpdateValidator validator = new UpdateValidator(); private IUpdateProvider provider; private CancellationTokenSource cts;
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this; DontDestroyOnLoad(gameObject); storage = new UpdateStorage(Path.Combine(Application.persistentDataPath, "SoulboundLegends", "LiveUpdates")); State = storage.LoadState(); provider = string.IsNullOrWhiteSpace(manifestBaseUrl) ? null : new HttpUpdateProvider(manifestBaseUrl);
        }
        private async void Start() { if (automaticCheckOnStart && provider != null) await CheckAndApplyAsync(); }
        public void SetProvider(IUpdateProvider newProvider) => provider = newProvider;
        public async Task<UpdateCheckResult> CheckAsync(CancellationToken token = default)
        {
            if (provider == null) return new UpdateCheckResult { compatible = false, reason = "No update provider configured." };
            try
            {
                var json = await provider.GetManifestJsonAsync(channel, token); var envelope = UpdateJson.FromJson<UpdateManifestEnvelope>(json); var m = envelope?.manifest;
                if (!validator.ValidateManifest(m, clientVersion, gameId, out var error)) return new UpdateCheckResult { compatible = false, reason = error, manifest = m };
                if (m.rolloutPercent < 100 && !IsInRollout(m.manifestId, m.rolloutPercent)) return new UpdateCheckResult { compatible = true, reason = "Outside rollout cohort.", manifest = m };
                bool newer = CompareVersions(m.contentVersion, State.contentVersion) > 0;
                if (newer) { AvailableManifest = m; LiveUpdateEvents.Discovered(m); }
                return new UpdateCheckResult { updateAvailable = newer, compatible = true, mandatory = newer && m.mandatory, manifest = m, reason = newer ? "Update available." : "Already current." };
            }
            catch (Exception e) { LiveUpdateEvents.Failed(e.Message); return new UpdateCheckResult { compatible = false, reason = e.Message }; }
        }
        public async Task<UpdateResult> CheckAndApplyAsync(CancellationToken token = default)
        {
            var check = await CheckAsync(token); if (!check.compatible || !check.updateAvailable) return new UpdateResult { success = !check.compatible ? false : true, message = check.reason, version = State.contentVersion };
            return await ApplyAsync(check.manifest, token);
        }
        public async Task<UpdateResult> ApplyAsync(LiveUpdateManifest manifest, CancellationToken token = default)
        {
            if (!validator.ValidateManifest(manifest, clientVersion, gameId, out var error)) return Fail(error);
            if (CompareVersions(manifest.contentVersion, State.contentVersion) <= 0) return new UpdateResult { success = true, message = "Already installed.", version = State.contentVersion };
            if (manifest.stage == UpdateReleaseStage.Paused || manifest.stage == UpdateReleaseStage.RolledBack || manifest.stage == UpdateReleaseStage.Retired) return Fail("This release is not active.");
            var previous = State; await storage.BackupStateAsync(previous);
            try
            {
                var next = new InstalledUpdateState { contentVersion = manifest.contentVersion, manifestId = manifest.manifestId, configJson = manifest.configJson, installedFiles = new List<string>(previous.installedFiles) };
                int count = Math.Max(1, manifest.files.Count), done = 0;
                foreach (var file in manifest.files)
                {
                    token.ThrowIfCancellationRequested(); byte[] data = await provider.DownloadFileAsync(file, token);
                    if (file.sizeBytes > 0 && data.LongLength != file.sizeBytes) throw new InvalidDataException("Size mismatch for " + file.id);
                    if (!UpdateValidator.VerifySha256(data, file.sha256)) throw new InvalidDataException("Checksum mismatch for " + file.id);
                    var path = storage.FilePath(file.relativePath); var temp = path + ".download"; await File.WriteAllBytesAsync(temp, data, token); if (File.Exists(path)) File.Delete(path); File.Move(temp, path); if (!next.installedFiles.Contains(file.relativePath)) next.installedFiles.Add(file.relativePath);
                    done++; LiveUpdateEvents.Progress((float)done / count);
                }
                State = next; await storage.SaveStateAsync(State); storage.ClearBackup(); LiveUpdateEvents.Validated(manifest); LiveUpdateEvents.Applied(manifest);
                return new UpdateResult { success = true, restartRequired = manifest.applyMode == UpdateApplyMode.RestartRequired, clientUpdateRequired = manifest.applyMode == UpdateApplyMode.ClientUpdateRequired, message = "Update applied successfully.", version = manifest.contentVersion };
            }
            catch (Exception e) { await RollbackAsync(); return Fail("Update failed and was rolled back: " + e.Message); }
        }
        public async Task<bool> RollbackAsync()
        {
            try { var backup = storage.LoadBackup(); if (backup == null) return false; State = backup; await storage.SaveStateAsync(State); storage.ClearBackup(); if (AvailableManifest != null) LiveUpdateEvents.RolledBack(AvailableManifest); return true; } catch { return false; }
        }
        public string GetConfigJson() => State?.configJson ?? "{}";
        public T GetConfig<T>() => UpdateJson.FromJson<T>(GetConfigJson());
        public bool IsFeatureEnabled(string key, bool fallback = false)
        {
            try { var root = UpdateJson.FromJson<FeatureFlags>(GetConfigJson()); if (root?.flags == null) return fallback; var x = root.flags.FirstOrDefault(v => v.key == key); return x == null ? fallback : x.enabled; } catch { return fallback; }
        }
        private UpdateResult Fail(string msg) { LiveUpdateEvents.Failed(msg); return new UpdateResult { success = false, message = msg, version = State.contentVersion }; }
        private static int CompareVersions(string a, string b) { Version.TryParse((a ?? "0.0.0").Split('-')[0].Split('+')[0], out var av); Version.TryParse((b ?? "0.0.0").Split('-')[0].Split('+')[0], out var bv); return av.CompareTo(bv); }
        private static bool IsInRollout(string id, int percent) { unchecked { int h = 17; foreach (char c in id ?? "") h = h * 31 + c; return Math.Abs(h % 100) < percent; } }
        [Serializable] private class FeatureFlags { public List<Flag> flags = new List<Flag>(); }
        [Serializable] private class Flag { public string key; public bool enabled; }
    }
}
