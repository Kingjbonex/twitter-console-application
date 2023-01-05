using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace TwitterCLI
{
    public class ConsoleService : BackgroundService
    {
        private readonly ILogger<ConsoleService> _logger;
        private TwitterCache _memCache;

        public IServiceProvider Services { get; }

        public ConsoleService(IServiceProvider services, TwitterCache memCache, ILogger<ConsoleService> logger)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            _memCache = memCache ?? throw new ArgumentNullException(nameof(memCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _ = Task.Run(() => SampleTweetsAsync(cancellationToken));

            string? tweetCount = "0";
            Dictionary<string, int> hashtags = null;

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

            AnsiConsole.Write(new FigletText("Twitter Stats").Centered().Color(Color.Purple));
            AnsiConsole.Status()
                .Start("Thinking...", ctx =>
                {
                    // Simulate some work
                    AnsiConsole.MarkupLine("Doing some work...");
                    Thread.Sleep(1000);

                    // Update the status and spinner
                    ctx.Status("Thinking some more");
                    ctx.Spinner(Spinner.Known.Star);
                    ctx.SpinnerStyle(Style.Parse("green"));

                    // Simulate some work
                    AnsiConsole.MarkupLine("Doing some more work...");
                    Thread.Sleep(2000);
                });

            await AnsiConsole.Live(table)
                .AutoClear(false)
                .Overflow(VerticalOverflow.Ellipsis)
                .Cropping(VerticalOverflowCropping.Bottom)
                .StartAsync(async ctx =>
                {
                    ctx.Refresh();
                    while (true)
                    {
                        try
                        {
                            tweetCount = _memCache.MemoryCache.Get("Tweet Count") == null ? "0" : Convert.ToString(_memCache.MemoryCache.Get("Tweet Count"));
                            table.Columns[1].Footer = new Text(tweetCount);

                            if(_memCache.MemoryCache.TryGetValue<Dictionary<string, int>>("Hashtags", out hashtags))
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
                });
            }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Console Service is stopping.");

            await base.StopAsync(cancellationToken);
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
}