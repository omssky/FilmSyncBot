using System.ComponentModel.DataAnnotations;

namespace FilmSyncBot.Options;

public class BotOptions
{
    public const string Configuration = "BotOptions";

    [Required(ErrorMessage = "Telegram bot token is null or empty.")]
    public string BotToken { get; set; } = "";
}