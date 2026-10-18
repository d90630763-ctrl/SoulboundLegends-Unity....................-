using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace SoulboundLegends.LiveUpdate
{
    public sealed class HttpUpdateProvider : IUpdateProvider
    {
        private readonly string manifestBaseUrl;
        private readonly HttpClient client = new HttpClient();
        public HttpUpdateProvider(string manifestBaseUrl) { this.manifestBaseUrl = manifestBaseUrl?.TrimEnd('/'); }
        public async Task<string> GetManifestJsonAsync(string channel, CancellationToken token)
        {
            var url = $"{manifestBaseUrl}/{channel}/manifest.json";
            using var r = await client.GetAsync(url, token); r.EnsureSuccessStatusCode(); return await r.Content.ReadAsStringAsync(token);
        }
        public async Task<byte[]> DownloadFileAsync(UpdateFile file, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(file.url)) throw new InvalidOperationException("Update file URL is empty.");
            using var r = await client.GetAsync(file.url, HttpCompletionOption.ResponseHeadersRead, token); r.EnsureSuccessStatusCode(); return await r.Content.ReadAsByteArrayAsync(token);
        }
    }
}
