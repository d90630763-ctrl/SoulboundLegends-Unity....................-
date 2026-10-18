using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace SoulboundLegends.LiveUpdate
{
    public sealed class LocalUpdateProvider : IUpdateProvider
    {
        private readonly string root;
        public LocalUpdateProvider(string rootFolder) { root = rootFolder; }
        public async Task<string> GetManifestJsonAsync(string channel, CancellationToken token)
        {
            var p = Path.Combine(root, channel, "manifest.json");
            return await File.ReadAllTextAsync(p, token);
        }
        public async Task<byte[]> DownloadFileAsync(UpdateFile file, CancellationToken token)
        {
            return await File.ReadAllBytesAsync(Path.Combine(root, file.relativePath), token);
        }
    }
}
