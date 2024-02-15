using EmailSenderTriger;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SendGrid;
using SendGrid.Extensions.DependencyInjection;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((hostContext,services) =>
    {
        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(config.GetSection("AzureWebJobsStorage")).WithName("Sender");
        });
        services.AddSendGrid(options =>
            options.ApiKey = config.GetSection("ApiKey").Get<string>()
        );
        services.Configure<EmailOptions>(config.GetSection("Email"));
 

    })
    .Build();

host.Run();
