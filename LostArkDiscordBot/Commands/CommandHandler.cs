using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkDiscordBot.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LostArkDiscordBot.Commands
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient client;
        private readonly InteractionService interactionService;
        private readonly IServiceProvider services;
        private readonly ILogger<CommandHandler> logger;
        private readonly bool isDevelopment;
        private readonly ulong? testGuildId;

        public CommandHandler(DiscordSocketClient client, InteractionService interactionService, IServiceProvider services, IHostEnvironment hostEnvironment, IOptions<LostArkBotOptions> options, ILogger<CommandHandler> logger)
        {
            this.client = client;
            this.interactionService = interactionService;
            this.services = services;
            this.logger = logger;
            isDevelopment = hostEnvironment.IsDevelopment();
            testGuildId = options.Value.TestGuildId;
        }

        public async Task InitializeAsync()
        {
            await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), services);
            client.Ready += RegisterCommandsAsync;
            client.InteractionCreated += HandleInteraction;
        }

        private async Task RegisterCommandsAsync()
        {
            if (isDevelopment && testGuildId.HasValue)
            {
                await interactionService.RegisterCommandsToGuildAsync(testGuildId.Value);
                return;
            }

            await interactionService.RegisterCommandsGloballyAsync();
        }

        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
                var ctx = new SocketInteractionContext(client, arg);
                await interactionService.ExecuteCommandAsync(ctx, services);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{typeof(CommandHandler)} error");

                // If a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                // response, or at least let the user know that something went wrong during the command execution.
                if(arg.Type == InteractionType.ApplicationCommand)
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}