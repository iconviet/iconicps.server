using Autofac;
using Autofac.Extensions.DependencyInjection;
using Equality.Client.Models;
using Equality.Client.Remote;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Serilog;
using Serilog.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using HttpClient = Equality.Client.Remote.HttpClient;

namespace Equality.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var control_level_switch = new LoggingLevelSwitch();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(control_level_switch)
                .Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
                .WriteTo.BrowserHttp(controlLevelSwitch: control_level_switch)
                .WriteTo.BrowserConsole()
                .CreateLogger();
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddMudServices();
            builder.RootComponents.Add<App>("app");
            builder.ConfigureContainer(new AutofacServiceProviderFactory(), ConfigureBuilder);
            builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            BackgroundJob.Start();
            await builder.Build().RunAsync();
        }

        public static Action<ContainerBuilder> ConfigureBuilder = builder =>
        {
            builder.RegisterType<JsonServiceClient>().PropertiesAutowired();
            builder.RegisterType<IconServiceClient>().PropertiesAutowired();
            builder.RegisterType<IconexExtension>().PropertiesAutowired().InstancePerLifetimeScope();
            builder.RegisterTypes(typeof(Program).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ViewModelBase))).ToArray()).PropertiesAutowired();
        };
    }
}