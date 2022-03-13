using Discord;
using Discord.WebSocket;

namespace LostArkDiscordBot.Configuration;

public class LostArkBotDiscordSocketConfig : DiscordSocketConfig
{
    public LostArkBotDiscordSocketConfig()
    {
        GatewayIntents = GetGatewayIntents();
    }

    private static GatewayIntents GetGatewayIntents()
    {
        return GatewayIntents.Guilds
               | GatewayIntents.GuildBans
               | GatewayIntents.GuildEmojis
               | GatewayIntents.GuildIntegrations
               //| GatewayIntents.GuildInvites
               //| GatewayIntents.GuildMembers
               | GatewayIntents.GuildMessages
               //| GatewayIntents.GuildPresences
               | GatewayIntents.GuildWebhooks
               | GatewayIntents.GuildMessageReactions
               | GatewayIntents.GuildMessageTyping
               //| GatewayIntents.GuildScheduledEvents
               | GatewayIntents.GuildVoiceStates
               | GatewayIntents.DirectMessages
               | GatewayIntents.DirectMessageReactions
               | GatewayIntents.DirectMessageTyping;
    }
}