using Telegram.Bot;
using Telegram.Bot.Polling;

namespace FilmSyncBot.Services;

public class ReceiverService(ITelegramBotClient botClient, IUpdateHandler updateHandler, ILogger<ReceiverService> logger)
{
    public async Task ReceiveAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = [],
            ThrowPendingUpdates = true
        };

        var me = await botClient.GetMeAsync(stoppingToken);
        logger.LogInformation("Start receiving updates for {BotName}", me.Username);

        await botClient.ReceiveAsync(updateHandler, receiverOptions, stoppingToken);
    }
}