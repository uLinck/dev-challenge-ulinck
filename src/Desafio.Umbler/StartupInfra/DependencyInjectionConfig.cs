using Desafio.Umbler.Features.DomainContext.Services;
using Desafio.Umbler.Features.DomainContext.Validators;
using Desafio.Umbler.Shared.Services.TldRegex;
using Desafio.Umbler.Shared.Services.WhoIs;
using DnsClient;
using Microsoft.Extensions.DependencyInjection;

namespace Desafio.Umbler.StartupInfra
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Singleton
            services.AddSingleton<ILookupClient>(new LookupClient());
            services.AddSingleton<IWhoIsClient, WhoIsClient>();
            services.AddSingleton<ITldRegexService, TldRegexService>();

            // Scoped
            services.AddScoped<ITldValidator, TldValidator>();
            services.AddScoped<IDomainValidator, DomainValidator>();
            services.AddScoped<IDomainService, DomainService>();

            return services;
        }
    }
}
