using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;
using Spectre.Console.Extensions.Hosting;

namespace TwitterCLI;

public class Program
{
    public static async Task Main(string[] args) =>
        await CreateHostBuilder(args)
                .RunConsoleAsync();

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseConsoleLifetime()
            .UseSpectreConsole(config =>
            {
                config.SetApplicationName("TwitterCLI");
                config.CaseSensitivity(CaseSensitivity.None);
                config.AddCommand<SampleCommand>("sample");
                config.AddExample(new[] { "sample", "--apikey", "4QcUzO1326POJv662KqMvy3Zc", "--secretkey", "lFoM3uYsWqfMMeHbFMqBHrgJfr4TJyN28o2HlVwVWiNVSeMeDt", "--token", "AAAAAAAAAAA" });
#if DEBUG
                config.PropagateExceptions();
                config.ValidateExamples();
#endif
            })
            .ConfigureServices((context, services) =>
            {
                services.AddMemoryCache();
                services.AddSingleton<ITwitterCache, TwitterCache>();
                services.AddSingleton<IScopedProcessingService, TwitterService>();
                services.AddSingleton<ITwitterClient, TwitterClient>();
            });
}