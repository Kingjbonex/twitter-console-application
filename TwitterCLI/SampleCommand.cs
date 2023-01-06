using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace TwitterCLI;

public sealed class SampleCommand : AsyncCommand<SampleCommand.Settings>
{
    private readonly IScopedProcessingService[] _backgroundServices;

    public sealed class Settings : CommandSettings
    {
        [CommandOption("--apikey <APIKEY>")]
        [Description("Call Twitter v2 Sample Stream and output statistics")]
        public string TwitterApiKey { get; set; }

        [CommandOption("--secretkey <SECRETKEY>")]
        [Description("Call Twitter v2 Sample Stream and output statistics")]
        public string TwitterSecretKey { get; set; }

        [CommandOption("--token <TOKEN>")]
        [Description("Call Twitter v2 Sample Stream and output statistics")]
        public string TwitterBearerToken { get; set; }


        [CommandOption("--time <TIME>")]
        [Description("Amount of time to gather data in minutes")]
        public string Time { get; set; }
    }

    public SampleCommand(IScopedProcessingService[] backgroundServices)
    {
        _backgroundServices = backgroundServices;
    }

    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        var timeInMinutes = new TimeSpan(0, Convert.ToInt16(settings.Time), 0);
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(timeInMinutes);

        foreach(var service in _backgroundServices)
        {
            await service.DoWorkAsync(cancellationTokenSource.Token);
        }

        return 0;
    }
}