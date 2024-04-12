using FilmSyncBot.Handlers;
using FilmSyncBot.Options;
using FilmSyncBot.Services;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    { 
        services.AddOptions<BotOptions>()
            .BindConfiguration(BotOptions.Configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddHttpClient("telegram_bot_client")
            .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
            {
                var botOptions = sp.GetRequiredService<IOptions<BotOptions>>();
                var options = new TelegramBotClientOptions(botOptions.Value.BotToken);
                return new TelegramBotClient(options, httpClient);
            });
                
        services.AddScoped<IUpdateHandler, UpdateHandler>();
        services.AddScoped<ReceiverService>();
        services.AddHostedService<PollingService>();
    })
    .Build();

await host.RunAsync();
