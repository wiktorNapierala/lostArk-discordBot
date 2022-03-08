namespace LostArkDiscordBot.Configuration;

public class LostArkBotOptions
{
    public const string SectionName = "LostArkBot";

    public string Nickname { get; set; } = "Lost Ark Bot";
    public string Token { get; set; } = string.Empty;
}