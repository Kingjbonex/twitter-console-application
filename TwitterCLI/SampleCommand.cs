using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using Microsoft.Extensions.Caching.Memory;
using System.Configuration;

namespace TwitterCLI;

[Description("Stream tweets from Twitter and output statistics on the incoming tweets.")]
public sealed class SampleCommand : AsyncCommandBase<SampleCommand.Settings>
{
    private readonly IScopedProcessingService _backgroundServices;
    private readonly ITwitterCache _twitterCache;
    private readonly ITwitterClient _twitterClient;

    public sealed class Settings : CommandSettings
    {
        [CommandOption("--apikey <APIKEY>")]
        [Description("Call Twitter v2 Sample Stream and output statistics")]
        public string? TwitterApiKey { get; set; }

        [CommandOption("--secretkey <SECRETKEY>")]
        [Description("Call Twitter v2 Sample Stream and output statistics")]
        public string? TwitterSecretKey { get; set; }

        [CommandOption("--token <TOKEN>")]
        [Description("Call Twitter v2 Sample Stream and output statistics")]
        public string? TwitterBearerToken { get; set; }

        [CommandOption("--time <TIME>")]
        [Description("Amount of time to gather data in minutes")]
        [DefaultValue("5")]
        public string? Time { get; set; }
    }

    public SampleCommand(IScopedProcessingService backgroundServices, ITwitterClient twitterClient, ITwitterCache twitterCache)
    {
        _backgroundServices = backgroundServices ?? throw new ArgumentNullException(nameof(backgroundServices));
        _twitterClient = twitterClient ?? throw new ArgumentNullException(nameof(twitterClient));
        _twitterCache = twitterCache ?? throw new ArgumentNullException(nameof(twitterCache));
    }

    protected override async Task<int> OnExecuteAsync(CommandContext context, Settings settings)
    {
        var timeInMinutes = new TimeSpan(0, Convert.ToInt16(settings.Time), 0);
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(timeInMinutes);


        if (settings.TwitterBearerToken != null && settings.TwitterSecretKey != null && settings.TwitterApiKey != null)
        {
            _twitterClient.ConfigureTwitterClient(settings);
        }
        else
        {
            throw new ConfigurationErrorsException("You need to call ");
        }
        _ = Task.Run(() => _backgroundServices.DoWorkAsync(cancellationTokenSource.Token));

        int tweetCount = 0;
        Dictionary<string, int>? hashtags = new Dictionary<string, int>();

        var table = new Table().Border(TableBorder.AsciiDoubleHead);
        table.AddColumn(new TableColumn(new Panel("[yellow]Hashtag Count[/]").BorderColor(Color.Blue)).Footer("Tweet Count"));
        table.AddColumn(new TableColumn(new Panel("[yellow]Hashtag[/]").BorderColor(Color.Blue)).Footer("0"));
        table.AddEmptyRow();
        table.AddEmptyRow();
        table.AddEmptyRow();
        table.AddEmptyRow();
        table.AddEmptyRow();
        table.AddEmptyRow();
        table.AddEmptyRow();
        table.AddEmptyRow();
        table.AddEmptyRow();
        table.AddEmptyRow();

        AnsiConsole.Status()
            .Start("Thinking...", ctx =>
            {
                AnsiConsole.MarkupLine("Starting up Twitter Sample Stream...");
                Thread.Sleep(1000);

                ctx.Status("Waiting for Sample Data to stream in...");
                ctx.Spinner(Spinner.Known.Star);
                ctx.SpinnerStyle(Style.Parse("green"));

                AnsiConsole.MarkupLine($"Running Sample Stream Statistics for `{timeInMinutes.Minutes}` minutes...");
                Thread.Sleep(2000);
            });

        await AnsiConsole.Live(table)
            .AutoClear(false)
            .Overflow(VerticalOverflow.Ellipsis)
            .Cropping(VerticalOverflowCropping.Bottom)
            .StartAsync(ctx =>
            {
                ctx.Refresh();
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        if (_twitterCache.MemoryCache.TryGetValue<int>(_twitterCache.TweetCountKey, out tweetCount))
                        {
                            table.Columns[1].Footer = new Text(tweetCount.ToString());
                        }

                        if (_twitterCache.MemoryCache.TryGetValue<Dictionary<string, int>>(_twitterCache.HashtagsKey, out hashtags))
                        {
                            var sortedList = from kvp in hashtags
                                             orderby kvp.Value descending
                                             select kvp;

                            int j = 0;
                            foreach (KeyValuePair<string, int> kvp in sortedList.Take(10))
                            {
                                table.UpdateCell(j, 0, new Text(Convert.ToString(kvp.Value)));
                                table.UpdateCell(j, 1, new Text(kvp.Key));
                                j++;
                            }
                        }
                        ctx.Refresh();
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.WriteException(ex);
                    }
                }
                return Task.CompletedTask;
            });

        return 0;
    }
}