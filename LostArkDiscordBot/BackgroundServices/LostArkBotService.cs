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
    private readonly CommandsHandler commandsHandler;
    private readonly DiscordSocketClient discordClient;
    private readonly LoggingHandler loggingHandler;
    private readonly IOptions<LostArkBotOptions> options;
    private readonly IBotVersionProvider versionProvider;

    public LostArkBotService(DiscordSocketClient discordClient, CommandsHandler commandsHandler,
        IBotVersionProvider versionProvider,
        LoggingHandler loggingHandler,
        IOptions<LostArkBotOptions> options,
        ILogger<LostArkBotService> logger)
        : base(logger)
    {
        this.discordClient = discordClient;
        this.commandsHandler = commandsHandler;
        this.versionProvider = versionProvider;
        this.loggingHandler = loggingHandler;
        this.options = options;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        discordClient.GuildAvailable += OnGuildAvailable;
        await discordClient.LoginAsync(TokenType.Bot, options.Value.Token);
        await commandsHandler.InitializeAsync();
        loggingHandler.Initialize();
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