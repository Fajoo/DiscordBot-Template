using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace DiscordBot;

class Program
{
    // Program entry point
    static async Task Main(string[] args)
    {
        var logging = new LoggerConfiguration()
            .MinimumLevel.Override("DiscordBot", LogEventLevel.Information)
            .WriteTo.File("DiscordBotLog-.txt", rollingInterval:
                RollingInterval.Day)
            .CreateLogger();

        var host = CreateHostBuilder(args).Build();

        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureDiscordHost((context, config) =>
            {
                config.SocketConfig = new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Info,
                    AlwaysDownloadUsers = true,
                    MessageCacheSize = 200
                };

                config.Token = context.Configuration["token"];
            })
            .UseSerilog((context, configuration) => configuration
                        .Enrich.FromLogContext()
                        .MinimumLevel.Information()
                        .WriteTo.Console(),
                    preserveStaticLogger: true)
            .UseCommandService((context, config) =>
            {
                config.DefaultRunMode = RunMode.Async;
                config.CaseSensitiveCommands = false;
            })
            .UseInteractionService((context, config) =>
            {
                config.LogLevel = LogSeverity.Info;
                config.UseCompiledLambda = true;
            })
            .ConfigureServices((context, services) =>
            {
                services.AddHostedService<CommandHandler>();

                //Add any other services here
                services.AddHttpClient();
                services.AddHostedService<BotStatusService>();
            });
}