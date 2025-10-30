using Microsoft.Extensions.DependencyInjection;

namespace Desafio.Umbler.StartupInfra
{
    public static class BlazorConfig
    {
        public static IServiceCollection AddBlazor(this IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddServerSideBlazor();
            services.AddRazorPages();

            return services;
        }
    }
}
