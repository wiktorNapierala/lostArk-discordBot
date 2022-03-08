using System.Reflection;
using Microsoft.Extensions.Hosting;

namespace LostArkDiscordBot.Configuration;

public class BotVersionProvider : IBotVersionProvider
{
    public BotVersionProvider(IHostEnvironment hostEnvironment)
    {
        var assembly = Assembly.GetEntryAssembly();
        var attribute = assembly?.GetCustomAttribute<AssemblyFileVersionAttribute>();

        if (attribute != null)
            BotVersion = attribute.Version;

        if (hostEnvironment.IsDevelopment())
            BotVersion = $"{BotVersion}-local";
    }

    public string BotVersion { get; } = "1.0";
}

public interface IBotVersionProvider
{
    string BotVersion { get; }
}