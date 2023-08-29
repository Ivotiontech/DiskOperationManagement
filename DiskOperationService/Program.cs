using DiskOperationService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
        services.AddHostedService<Worker>();
    })
    .UseWindowsService()
    .Build();

await host.RunAsync();
