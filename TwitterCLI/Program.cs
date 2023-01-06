using Autofac.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Extensions.Hosting;
using System.Configuration;
using Tweetinvi;
using Tweetinvi.Streaming.V2;

namespace TwitterCLI
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .UseConsoleLifetime()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddUserSecrets<Program>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMemoryCache();
                    services.AddSingleton<ITwitterCache, TwitterCache>();
                    services.AddScoped<IScopedProcessingService, TwitterService>();
                    services.AddScoped<IScopedProcessingService, ConsoleService>();
                    services.TryAddScoped<ISampleStreamV2>(s =>
                    {
                        return s.GetService<TwitterClient>()!.StreamsV2.CreateSampleStream();
                    });
                    services.TryAddTransient<Tweetinvi.Models.IReadOnlyConsumerCredentials>(s =>
                    {
                        string? twitterKey = hostContext.Configuration.GetValue<string>("twitter_api_key");
                        string? secretKey = hostContext.Configuration.GetValue<string>("twitter_secret_key");
                        string? bearerToken = hostContext.Configuration.GetValue<string>("twitter_bearer_token");
                        if (twitterKey != null && secretKey != null && bearerToken != null)
                        {
                            return new Tweetinvi.Models.ConsumerOnlyCredentials(twitterKey, secretKey)
                            {
                                BearerToken = bearerToken,
                            };
                        }
                        else
                        {
                            throw new ConfigurationErrorsException("User Secrets have not been set. Please refer to the README.md to set up secrets for this application.");
                        }
                    });
                    services.TryAddTransient<TwitterClient>(s =>
                    {
                        return new TwitterClient(s.GetService<Tweetinvi.Models.IReadOnlyConsumerCredentials>());
                    });
                })
                .UseSpectreConsole<SampleCommand>(config =>
                {
                    config.SetApplicationName("Twitter Stats CLI");
                    config.ValidateExamples();

                    // Sample
                    config.AddCommand<SampleCommand>("sample")
                    .WithDescription("Call Twitter v2 Sample Stream and output statistics")
                    .WithExample(new[] { "sample", "--apikey", "XXXXX", "--secretkey", "XXXXX", "--token", "XXXXX" });

                    AnsiConsole.Write(new FigletText("Twitter Stats CLI").Centered().Color(Color.Purple));
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration);
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .RunConsoleAsync();

            return Environment.ExitCode;
        }
    }
}