using Desafio.Umbler.Persistence;
using Desafio.Umbler.Features.DomainContext.Services;
using Desafio.Umbler.Features.DomainContext.Validators;
using Desafio.Umbler.Shared.Services.WhoIs;
using Desafio.Umbler.Shared.Services.TldRegex;
using DnsClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace Desafio.Umbler
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
            services.AddDbContext<DatabaseContext>(
                dbContextOptions => dbContextOptions
                    .UseMySql(connectionString, serverVersion)
            );

            services.AddSingleton<ILookupClient>(new LookupClient());
            services.AddSingleton<IWhoIsClient, WhoIsClient>();
            services.AddSingleton<ITldRegexService, TldRegexService>();

            services.AddScoped<ITldValidator, TldValidator>();
            services.AddScoped<IDomainValidator, DomainValidator>();
            services.AddScoped<IDomainService, DomainService>();

            services.AddControllersWithViews();
            services.AddServerSideBlazor();
            services.AddRazorPages();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Umbler API Challenge",
                    Version = "v1",
                    Description = "API to query DNS and WHOIS information for domains"
                });

                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));

            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Desafio Umbler API V1");
                    c.RoutePrefix = "swagger";
                });
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}