using Spectre.Console.Cli;
using Spectre.Console;

namespace TwitterCLI;

public abstract class AsyncCommandBase<TSettings> : AsyncCommand<TSettings> where TSettings : CommandSettings
{
    public override async Task<int> ExecuteAsync(CommandContext context, TSettings settings)
    {
        try
        {
            return await OnExecuteAsync(context, settings);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenPaths);
            return -1;
        }
    }

    protected abstract Task<int> OnExecuteAsync(CommandContext context, TSettings settings);
}