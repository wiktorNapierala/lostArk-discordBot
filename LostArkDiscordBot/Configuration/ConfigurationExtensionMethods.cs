using Discord.Interactions;
using Discord.WebSocket;
using LostArkDiscordBot.BackgroundServices;
using LostArkDiscordBot.Commands;
using LostArkDiscordBot.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LostArkDiscordBot.Configuration;

public static class ConfigurationExtensionMethods
{
    public static IHostBuilder ConfigureBotServices(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureServices((context, collection) =>
        {
            collection.AddLogging();
            collection.AddDiscordBotService(context);
        });
    }

    private static void AddDiscordBotService(this IServiceCollection collection, HostBuilderContext context)
    {
        collection.Configure<LostArkBotOptions>(context.Configuration.GetSection(LostArkBotOptions.SectionName));
        collection.AddSingleton<DiscordSocketConfig, LostArkBotDiscordSocketConfig>();
        collection.AddSingleton<LostArkBotInteractionServiceConfig>();
        collection.AddSingleton<DiscordSocketClient>();
        collection.AddSingleton(services => new InteractionService(
            services.GetRequiredService<DiscordSocketClient>(),
            services.GetRequiredService<LostArkBotInteractionServiceConfig>()));
        collection.AddSingleton<IBotVersionProvider, BotVersionProvider>();
        collection.AddSingleton<LoggingHandler>();
        collection.AddSingleton<CommandHandler>();
        collection.AddHostedService<LostArkBotService>();
    }
}