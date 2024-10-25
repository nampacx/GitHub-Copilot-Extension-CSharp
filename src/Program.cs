using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
     .ConfigureAppConfiguration((context, config) =>
     {
         config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
     })
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

    })
    .Build();

host.Run();