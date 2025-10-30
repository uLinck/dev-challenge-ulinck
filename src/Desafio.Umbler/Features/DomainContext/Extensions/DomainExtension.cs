using Desafio.Umbler.Persistence.Models;
using Desafio.Umbler.Features.DomainContext.Dto;

namespace Desafio.Umbler.Features.DomainContext.Extensions
{
    public static class DomainExtension
    {
        public static DomainDto ToDto(this Domain domain)
        {
            return new DomainDto(
                Name: domain.Name,
                Ip: domain.Ip,
                HostedAt: domain.HostedAt
            );
        }
    }
}
