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
            .ConfigureServices((hostingContext, servcies) =>
            {
                var configuration = hostingContext.Configuration;

                servcies.ConfigureJwt(configuration);

                servcies.AddOcelot();
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
                app.UseOcelot().Wait();
            })
            .Build()
            .Run();
        }
    }
}
