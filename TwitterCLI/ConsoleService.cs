using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace TwitterCLI;

public class ConsoleService : IScopedProcessingService
{
    private readonly ILogger<ConsoleService> _logger;
    private ITwitterCache _memCache;

    public IServiceProvider Services { get; }

    public ConsoleService(IServiceProvider services, ITwitterCache memCache, ILogger<ConsoleService> logger)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        _memCache = memCache ?? throw new ArgumentNullException(nameof(memCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        _ = Task.Run(() => SampleTweetsAsync(cancellationToken));

        int tweetCount = 0;
        Dictionary<string, int>? hashtags = new Dictionary<string, int>();

        var table = new Table().Expand().BorderColor(Color.Grey);
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

                AnsiConsole.MarkupLine("Twitter Sample Data is streaming in...");
                Thread.Sleep(2000);
            });

        await AnsiConsole.Live(table)
            .AutoClear(false)
            .Overflow(VerticalOverflow.Ellipsis)
            .Cropping(VerticalOverflowCropping.Bottom)
            .StartAsync(ctx =>
            {
                ctx.Refresh();
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        if (_memCache.MemoryCache.TryGetValue<int>(_memCache.TweetCountKey, out tweetCount))
                        {
                            table.Columns[1].Footer = new Text(tweetCount.ToString());
                        }

                        if (_memCache.MemoryCache.TryGetValue<Dictionary<string, int>>(_memCache.HashtagsKey, out hashtags))
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
                        _logger.LogError(ex, "An unexpected error occurred while attempting to refresh the console.");
                    }
                }
                return Task.CompletedTask;
            });
    }

    private Task SampleTweetsAsync(CancellationToken stoppingToken)
    {
        using (var scope = Services.CreateScope())
        {
            var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IScopedProcessingService>();

            return scopedProcessingService.DoWorkAsync(stoppingToken);
        }
    }
}