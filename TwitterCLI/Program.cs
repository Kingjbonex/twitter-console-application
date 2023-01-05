using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = new HostBuilder()
               .ConfigureHostConfiguration(configHost =>
               {
                   configHost.SetBasePath(Directory.GetCurrentDirectory());
                   configHost.AddUserSecrets<Program>();
               })
               .ConfigureServices((services) =>
               {
                   var serviceProvider = services.BuildServiceProvider();
                   var config = serviceProvider.GetService<IConfiguration>();
                   string twitterKey = config.GetValue<string>("twitter_api_key"); // TODO: Handle Null values from configs to avoid failures down the line calling TwitterClient
                   string secretKey = config.GetValue<string>("twitter_secret_key");
                   string bearerToken = config.GetValue<string>("twitter_bearer_token");
                   services.AddMemoryCache();
                   services.AddSingleton<TwitterCache>();
                   services.AddScoped<IScopedProcessingService, TwitterService>();
                   services.AddHostedService<ConsoleService>();
                   services.TryAddScoped<ISampleStreamV2>(s =>
                   {
                       return s.GetService<TwitterClient>().StreamsV2.CreateSampleStream();
                   });
                   services.TryAddTransient<Tweetinvi.Models.IReadOnlyConsumerCredentials>(s =>
                   {
                       return new Tweetinvi.Models.ConsumerOnlyCredentials(twitterKey, secretKey)
                       {
                           BearerToken = bearerToken,
                       };
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