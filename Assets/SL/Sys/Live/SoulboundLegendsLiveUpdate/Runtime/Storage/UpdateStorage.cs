using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
namespace SoulboundLegends.LiveUpdate
{
    public sealed class UpdateStorage
    {
        public string Root { get; }
        public string StatePath => Path.Combine(Root, "installed_state.json");
        public string BackupPath => Path.Combine(Root, "backup_state.json");
        public UpdateStorage(string root) { Root = root; Directory.CreateDirectory(Root); }
        public InstalledUpdateState LoadState()
        {
            try { if (!File.Exists(StatePath)) return new InstalledUpdateState(); return UpdateJson.FromJson<InstalledUpdateState>(File.ReadAllText(StatePath)) ?? new InstalledUpdateState(); }
            catch { return new InstalledUpdateState(); }
        }
        public async Task SaveStateAsync(InstalledUpdateState state) { await File.WriteAllTextAsync(StatePath, UpdateJson.ToJson(state)); }
        public async Task BackupStateAsync(InstalledUpdateState state) { await File.WriteAllTextAsync(BackupPath, UpdateJson.ToJson(state)); }
        public bool HasBackup => File.Exists(BackupPath);
        public InstalledUpdateState LoadBackup() { return HasBackup ? UpdateJson.FromJson<InstalledUpdateState>(File.ReadAllText(BackupPath)) : null; }
        public void ClearBackup() { if (File.Exists(BackupPath)) File.Delete(BackupPath); }
        public string FilePath(string relative) { var full = Path.GetFullPath(Path.Combine(Root, relative)); var basePath = Path.GetFullPath(Root) + Path.DirectorySeparatorChar; if (!full.StartsWith(basePath, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Unsafe update path."); Directory.CreateDirectory(Path.GetDirectoryName(full)); return full; }
    }
}
