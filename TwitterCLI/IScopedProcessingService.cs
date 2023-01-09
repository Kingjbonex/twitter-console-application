namespace TwitterCLI;

public interface IScopedProcessingService
{
    Task DoWorkAsync(CancellationToken stoppingToken);
}
