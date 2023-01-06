using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Configuration;
using Tweetinvi;
using Tweetinvi.Streaming.V2;
using TwitterCLI;

namespace TwitterCLI
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

            await host.StartAsync();

            return Environment.ExitCode;
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = new HostBuilder()
               .ConfigureHostConfiguration(configHost =>
               {
                   configHost.SetBasePath(Directory.GetCurrentDirectory());
                   configHost.AddUserSecrets<Program>();
               })
               .ConfigureServices((hostContext, services) =>
               {
                   string? twitterKey = hostContext.Configuration.GetValue<string>("twitter_api_key");
                   string? secretKey = hostContext.Configuration.GetValue<string>("twitter_secret_key");
                   string? bearerToken = hostContext.Configuration.GetValue<string>("twitter_bearer_token");
                   services.AddMemoryCache();
                   services.AddSingleton<ITwitterCache, TwitterCache>();
                   services.AddScoped<IScopedProcessingService, TwitterService>();
                   services.AddHostedService<ConsoleService>();
                   services.TryAddScoped<ISampleStreamV2>(s =>
                   {
                       return s.GetService<TwitterClient>()!.StreamsV2.CreateSampleStream();
                   });
                   services.TryAddTransient<Tweetinvi.Models.IReadOnlyConsumerCredentials>(s =>
                   {
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
               .ConfigureLogging((hostingContext, logging) =>
               {
                   logging.AddConfiguration(hostingContext.Configuration);
                   logging.AddConsole();
                   logging.AddDebug();
               });

            builder.UseConsoleLifetime();
            return builder;
        }
    }
}