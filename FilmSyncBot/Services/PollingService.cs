namespace FilmSyncBot.Services;

public class PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var receiver = scope.ServiceProvider.GetRequiredService<ReceiverService>();

                await receiver.ReceiveAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Handling updates failed");

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}