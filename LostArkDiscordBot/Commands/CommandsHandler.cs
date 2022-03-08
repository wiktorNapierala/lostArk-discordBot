using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace LostArkDiscordBot.Commands;

public class CommandsHandler
{
    private readonly CommandService commandService;
    private readonly DiscordSocketClient discordClient;
    private readonly IServiceProvider serviceProvider;

    public CommandsHandler(CommandService commandService, DiscordSocketClient discordClient,
        IServiceProvider serviceProvider)
    {
        this.commandService = commandService;
        this.discordClient = discordClient;
        this.serviceProvider = serviceProvider;
    }

    public async Task InitializeAsync()
    {
        discordClient.MessageReceived += MessageReceivedAsync;
        await commandService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);
    }

    private async Task MessageReceivedAsync(SocketMessage rawMessage)
    {
        // Ignore system messages, or messages from other bots
        if (rawMessage is not SocketUserMessage { Source: MessageSource.User } message)
            return;

        // This value holds the offset where the prefix ends
        var argPos = 0;
        // Perform prefix check. You may want to replace this with
        // (!message.HasCharPrefix('!', ref argPos))
        // for a more traditional command format like !help.
        if (!message.HasMentionPrefix(discordClient.CurrentUser, ref argPos))
            return;

        var context = new SocketCommandContext(discordClient, message);
        // Perform the execution of the command. In this method,
        // the command service will perform precondition and parsing check
        // then execute the command if one is matched.
        await commandService.ExecuteAsync(context, argPos, serviceProvider);
        // Note that normally a result will be returned by this format, but here
        // we will handle the result in CommandExecutedAsync,
    }
}