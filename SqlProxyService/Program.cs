using SqlProxyService.Services;
using SqlProxyService.Settings;

namespace SqlProxyService
{
    class Program
    {
        private const string ServiceName = "SQC";
        private static Setting settings;

        static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile($"appsettings.json")
                 .Build();

            settings = new Setting(configuration);

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {

            return Host.CreateDefaultBuilder(args)
                  .UseWindowsService(options =>
                  {
                      options.ServiceName = $"{ServiceName} Proxy Server";
                  })
                  .ConfigureAppConfiguration((hostingContext, configuration) =>
                  {
                      IHostEnvironment env = hostingContext.HostingEnvironment;

                      configuration
                           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                           .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                           .AddEnvironmentVariables();
                  })
                  .ConfigureServices((hostContext, services) =>
                  {
                      services.AddHostedService<SqlHttpWorker>();

                      services.AddSingleton(provider => settings.ServerOption);
                  });
        }
    }

}
