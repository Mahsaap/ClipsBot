using ClipsBot.Ignore;
using ClipsBot.Preconditions;
using ClipsBot.Services;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace ClipsBot
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).RunConsoleAsync();
        }
        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), false);
            })
            .ConfigureLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Information);
            })
            .ConfigureServices(services =>
            {
                services.AddTwitchLibApi(TwitchCreds.ClientId, TwitchCreds.ClientSecret, 800);
                services.AddHostedService<DiscordClient>();
                services.AddSingleton(new CommandService(new CommandServiceConfig
                {
                    ThrowOnError = true,
                    CaseSensitiveCommands = false
                }));
            })
            .UseConsoleLifetime();
    }
}

