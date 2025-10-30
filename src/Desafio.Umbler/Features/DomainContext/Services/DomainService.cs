using Desafio.Umbler.Persistence;
using Desafio.Umbler.Persistence.Models;
using Desafio.Umbler.Features.DomainContext.Dto;
using Desafio.Umbler.Features.DomainContext.Extensions;
using Desafio.Umbler.Features.DomainContext.Validators;
using Desafio.Umbler.Shared.Services.WhoIs;
using Desafio.Umbler.Shared.Results;
using DnsClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Desafio.Umbler.Features.DomainContext.Services
{
    public class DomainService : IDomainService
    {
        private readonly DatabaseContext _db;
        private readonly ILookupClient _lookup;
        private readonly ILogger<DomainService> _logger;
        private readonly IWhoIsClient _whoIsClient;

        public DomainService(DatabaseContext db, ILookupClient lookup, ILogger<DomainService> logger, IWhoIsClient whoIsClient)
        {
            _db = db;
            _lookup = lookup;
            _logger = logger;
            _whoIsClient = whoIsClient;
        }

        public async Task<ServiceResult<DomainDto>> GetAsync(string domainName)
        {
            _logger.LogInformation("Validando coerência do domínio: '{DomainName}'", domainName);
            var (isValid, error) = DomainValidator.Validate(domainName);
            
            if (!isValid)
            {
                _logger.LogWarning("Validação falhou para o domínio '{DomainName}': {Error}", domainName, error);
                return Result.ValidationError<DomainDto>(error);
            }

            domainName = domainName.Trim().ToLowerInvariant();

            var domain = await _db.Domains.FirstOrDefaultAsync(d => d.Name == domainName);

            var isCacheValid = domain != null && !IsDomainStale(domain);

            if (isCacheValid)
                return Result.Ok(domain.ToDto());

            try
            {
                var whoIsResult = await FetchWhoIsAsync(domainName);

                if (domain is null)
                {
                    domain = new Domain(domainName, whoIsResult);

                    _db.Domains.Add(domain);
                }
                else
                {
                    domain.Update(domainName, whoIsResult);
                }

                await _db.SaveChangesAsync();

                return Result.Ok(domain.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar informações do domínio '{DomainName}'", domainName);
                return Result.Error<DomainDto>(
                    $"Erro ao buscar informações do domínio '{domainName}'. " +
                    "Verifique se o domínio existe e está acessível.");
            }
        }

        private static bool IsDomainStale(Domain domain)
        {
            var ttlTimeSpan = TimeSpan.FromSeconds(domain.Ttl);
            var timeSinceUpdate = DateTime.UtcNow - domain.UpdatedAt;
            return timeSinceUpdate > ttlTimeSpan;
        }

        private async Task<WhoIsResult> FetchWhoIsAsync(string domainName)
        {
            var WhoIs = await _whoIsClient.QueryAsync(domainName);

            var dnsResult = await _lookup.QueryAsync(domainName, QueryType.A);
            var aRecord = dnsResult.Answers.ARecords().FirstOrDefault();
            
            var ip = aRecord?.Address?.ToString();
            var ttl = aRecord?.TimeToLive ?? 3600;

            string hostedAt = null;
            if (!string.IsNullOrEmpty(ip))
            {
                var hostResponse = await _whoIsClient.QueryAsync(ip);
                hostedAt = hostResponse.OrganizationName;
            }

            return new WhoIsResult
            {
                WhoIs = WhoIs.Raw,
                Ip = ip,
                Ttl = ttl,
                HostedAt = hostedAt
            };
        }
    }

    public interface IDomainService
    {
        Task<ServiceResult<DomainDto>> GetAsync(string domainName);
    }
}
