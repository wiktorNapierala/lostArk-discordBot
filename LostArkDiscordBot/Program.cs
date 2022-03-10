using System.Reflection;
using LostArkDiscordBot.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var app = CreateHostBuilder(args).Build();

await app.RunAsync();

static IHostBuilder CreateHostBuilder(string[] args)
{
    var exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    return Host.CreateDefaultBuilder(args)
        .UseSystemd()
        .UseWindowsService()
        .ConfigureHostConfiguration(config =>
        {
            config.AddEnvironmentVariables();
            config.AddCommandLine(args);
        })
        .ConfigureAppConfiguration((context, builder) =>
        {
            builder.SetBasePath(exeDirectory)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
        })
        .ConfigureBotServices();
}