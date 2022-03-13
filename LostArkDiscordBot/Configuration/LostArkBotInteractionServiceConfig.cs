using Discord.Interactions;

namespace LostArkDiscordBot.Configuration;

public class LostArkBotInteractionServiceConfig : InteractionServiceConfig
{
    public LostArkBotInteractionServiceConfig()
    {
        DefaultRunMode = RunMode.Async;
    }
}