using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Desafio.Umbler.Features.DomainContext.Dto;
using Desafio.Umbler.Features.DomainContext.Services;

namespace Desafio.Umbler.Controllers
{
    [Route("api/domains")]
    [ApiController]
    public class DomainController : BaseController
    {
        private readonly IDomainService _domainService;

        public DomainController(IDomainService domainService)
        {
            _domainService = domainService;
        }

        /// <summary>
        /// Gets DNS and WHOIS information for a domain
        /// </summary>
        /// <param name="domainName">Domain name (e.g.: umbler.com)</param>
        /// <returns>Domain information including IP, Name Servers and Hosting</returns>
        [HttpGet, Route("{domainName}")]
        public async Task<ActionResult<ApiResponse<DomainDto>>> GetAsync(string domainName)
        {
            var result = await _domainService.GetAsync(domainName);
            return ToResponse(result);
        }
    }
}
