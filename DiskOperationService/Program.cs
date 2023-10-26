using LegalDLPBeta;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext,services) =>
    {
        System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
        IConfiguration configuration = hostContext.Configuration;
        services.Configure<ServerConfigModel>(configuration.GetSection(nameof(ServerConfigModel)));
        services.Configure<ServerConfigURL>(configuration.GetSection(nameof(ServerConfigURL)));
        services.AddHostedService<Worker>();
    })
    .UseWindowsService()
    .Build();

await host.RunAsync();
