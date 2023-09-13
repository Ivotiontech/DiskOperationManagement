using DiskOperationService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext,services) =>
    {
        System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
        IConfiguration configuration = hostContext.Configuration;
        services.Configure<ServerConfigModel>(configuration.GetSection(nameof(ServerConfigModel)));
        services.AddHostedService<Worker>();
    })
    .UseWindowsService()
    .Build();

await host.RunAsync();
