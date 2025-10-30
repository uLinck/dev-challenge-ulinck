using System.Threading.Tasks;
using Whois.NET;

namespace Desafio.Umbler.Shared.Services.WhoIs
{
    public class WhoIsClient : IWhoIsClient
    {
        public async Task<WhoisResponse> QueryAsync(string query)
        {
            return await WhoisClient.QueryAsync(query);
        }
    }

    public interface IWhoIsClient
    {
        Task<WhoisResponse> QueryAsync(string query);
    }
}
