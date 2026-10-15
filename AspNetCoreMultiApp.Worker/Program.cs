
using AspNetCoreMultiApp.Worker.Jobs;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaulConnection' not found");

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? throw new InvalidOperationException("ApiSettings:BaseUrl is not found");

builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHangfire(configuration =>
{
    configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(connectionString, new Hangfire.SqlServer.SqlServerStorageOptions
        {
            PrepareSchemaIfNecessary = true,
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        });
});

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 5;
    options.Queues = new[] { "default" };
});

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "MultiApp Service Worker";
});

builder.Services.AddScoped<ProductReportJob>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var recurringManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    recurringManager.AddOrUpdate<ProductReportJob>(
        "product-report",
        job => job.ExecuteAsync(),
        Cron.Minutely
    );
}

Console.WriteLine("ASP NET CORE HANGFIRE WORKER");
Console.WriteLine("Worker is running");
Console.WriteLine($"API:{apiBaseUrl}");
Console.WriteLine("Press Ctrl+C to stop");

await host.RunAsync();