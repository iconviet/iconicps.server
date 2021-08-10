using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;
using Stl.Fusion;
using WebMarkupMin.AspNetCore3;

namespace Equality.Server
{
    public class Startup
    {
        public void Configure(IApplicationBuilder application, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                application.UseWebAssemblyDebugging();
                application.UseDeveloperExceptionPage();
            }
            else
            {
                application.UseHsts();
            }
            application.UseHttpsRedirection();
            application.UseBlazorFrameworkFiles();
            application.UseStaticFiles();
            application.UseRouting();
            application.UseWebMarkupMin();
            application.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapControllers();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFusion();
            services.AddMudServices();
            services.AddControllers();
            services.AddDataProtection()
                .UseEphemeralDataProtectionProvider();
            services
                .AddServerSideBlazor()
                .AddCircuitOptions(x => x.DetailedErrors = true);
            services
                .AddHttpContextAccessor()
                .Configure<HubOptions>(x => x.EnableDetailedErrors = true)
                .AddWebMarkupMin(x =>
                {
                    x.DisablePoweredByHttpHeaders = true;
                    x.AllowMinificationInDevelopmentEnvironment = true;
                }).AddHtmlMinification(x => x.MinificationSettings.RemoveHtmlComments = false);
            services.AddRazorPages(x => x.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute()));
        }
    }
}