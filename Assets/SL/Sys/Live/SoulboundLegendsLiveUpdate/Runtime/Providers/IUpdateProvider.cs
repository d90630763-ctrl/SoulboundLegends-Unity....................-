using System.Threading;
using System.Threading.Tasks;
namespace SoulboundLegends.LiveUpdate
{
    public interface IUpdateProvider
    {
        Task<string> GetManifestJsonAsync(string channel, CancellationToken token);
        Task<byte[]> DownloadFileAsync(UpdateFile file, CancellationToken token);
    }
}
