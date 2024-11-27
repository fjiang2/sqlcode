using SqlProxyService.Services;
using SqlProxyService.Settings;

namespace SqlProxyService
{
    class StartUp
    {
        public const string ServiceName = "SQC";

        private readonly IConfiguration configuration;

        public StartUp()
        {
            configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile($"appsettings.json")
                 .Build();
        }

        public void Run(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public IHostBuilder CreateHostBuilder(string[] args)
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
                      // services.AddSingleton<ISetting>(provider => new Setting(configuration));
                      services.AddSingleton(provider => new Setting(configuration).ServerOption);

                      services.AddHostedService<SqlHttpWorker>();
                  });
        }
    }

}
