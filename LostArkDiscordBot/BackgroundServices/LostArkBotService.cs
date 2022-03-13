using Discord;
using Discord.WebSocket;
using LostArkDiscordBot.Commands;
using LostArkDiscordBot.Configuration;
using LostArkDiscordBot.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LostArkDiscordBot.BackgroundServices;

public class LostArkBotService : BackgroundServiceWithLogging
{
    private readonly CommandHandler commandHandler;
    private readonly DiscordSocketClient discordClient;
    private readonly LoggingHandler loggingHandler;
    private readonly IOptions<LostArkBotOptions> options;
    private readonly IBotVersionProvider versionProvider;

    public LostArkBotService(DiscordSocketClient discordClient,
        IBotVersionProvider versionProvider,
        LoggingHandler loggingHandler,
        IOptions<LostArkBotOptions> options,
        ILogger<LostArkBotService> logger,
        CommandHandler commandHandler)
        : base(logger)
    {
        this.discordClient = discordClient;
        this.versionProvider = versionProvider;
        this.loggingHandler = loggingHandler;
        this.options = options;
        this.commandHandler = commandHandler;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await commandHandler.InitializeAsync();
        discordClient.GuildAvailable += OnGuildAvailable;
        loggingHandler.Initialize();
        await discordClient.LoginAsync(TokenType.Bot, options.Value.Token);
        await base.StartAsync(cancellationToken);
    }

    private async Task OnGuildAvailable(SocketGuild guild)
        => await guild.CurrentUser.ModifyAsync(props => props.Nickname = options.Value.Nickname);

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await discordClient.StopAsync();
        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteServiceAsync(CancellationToken stoppingToken)
    {
        await discordClient.StartAsync();
        await discordClient.SetGameAsync($"v{versionProvider.BotVersion}");
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override void Dispose()
    {
        discordClient.Dispose();
        base.Dispose();
    }
}