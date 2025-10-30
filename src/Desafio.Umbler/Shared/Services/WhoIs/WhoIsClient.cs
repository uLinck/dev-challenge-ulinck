using System.Threading.Tasks;
using Whois.NET;

namespace Desafio.Umbler.Shared.Services.WhoIs
{
    public class WhoIsClient : IWhoIsClient
    {
        public Task<WhoisResponse> QueryAsync(string query)
        {
            return WhoisClient.QueryAsync(query);
        }
    }

    public interface IWhoIsClient
    {
        Task<WhoisResponse> QueryAsync(string query);
    }
}
