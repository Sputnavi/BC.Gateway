using BC.Gateway.Helpers;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace BC.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new WebHostBuilder()
            .UseKestrel()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config
                    .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                    .AddJsonFile(Path.Combine("Configurations", $"configuration.{hostingContext.HostingEnvironment.EnvironmentName}.json"), true, true)
                    .AddEnvironmentVariables();
            })
            .ConfigureServices((hostingContext, services) =>
            {
                var configuration = hostingContext.Configuration;

                services.ConfigureJwt(configuration);
                services.ConfigureCorsPolicy();
                services.AddOcelot();
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                if (hostingContext.HostingEnvironment.IsDevelopment())
                {
                    logging.AddConsole();
                }
            })
            .UseIISIntegration()
            .Configure(app =>
            {
                app.UseCors("CorsPolicy");
                app.UseOcelot().Wait();
            })
            .Build()
            .Run();
        }
    }
}
