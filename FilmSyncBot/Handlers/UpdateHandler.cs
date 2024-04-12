using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace FilmSyncBot.Handlers;

public class UpdateHandler(ITelegramBotClient botClient, ILogger<UpdateHandler> logger) : IUpdateHandler
{
    public async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
    {
        var handler = update switch
        {
            { Message.Text: "/start" } => HandleStartCommand(update.Message, cancellationToken),
            _ => UnknownUpdateHandlerAsync(update, cancellationToken)
        };

        await handler;
    }

    private async Task HandleStartCommand(Message message, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received /start command from user {UserName}({UserId})",
            message.From!.Username,
            message.From!.Id);

        try
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Hi telegram",
                cancellationToken: cancellationToken
            );
        }
        catch (Exception ex)
        {
            logger.LogError("Error handling /start command: {ErrorMessage}", ex.Message);
        }
    }

    private Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
    {
        logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient client, Exception exception,
        CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException =>
                $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        logger.LogInformation("HandleError: {ErrorMessage}", errorMessage);

        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }
}