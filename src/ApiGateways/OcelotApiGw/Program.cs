using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;
using Common.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(SeriLogger.Configure);

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile($"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true);
});
//.ConfigureLogging((hostingContext, loggingBuilder) =>
//{
//    loggingBuilder.ClearProviders();
//    loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
//    loggingBuilder.AddConsole();
//    loggingBuilder.AddDebug();
//});

builder.Services.AddOcelot().AddCacheManager(settings => settings.WithDictionaryHandle());


var app = builder.Build();

await app.UseOcelot();

app.MapGet("/", () => "Hello World!");

app.Run();
