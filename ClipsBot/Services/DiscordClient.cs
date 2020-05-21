using ClipsBot.Ignore;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClipsBot.Services
{
    public class DiscordClient : IHostedService
    {
        public DiscordSocketClient Client { get; private set; }

        private readonly ILogger<DiscordClient> _logger;

        public DiscordClient(ILogger<DiscordClient> logger)
        {
            _logger = logger;
        }

        public async Task RunAsync()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig { LogLevel = LogSeverity.Debug, MessageCacheSize = 100 });

            _logger.LogInformation("Client Created!");

            Client.Log += Client_Log;
            Client.MessageReceived += Client_MessageReceived;

            await Client.LoginAsync(TokenType.Bot, Token.token);
            await Client.StartAsync();

            _logger.LogInformation("Discord Client Started!");

            await Task.Delay(-1);
        }

        private async Task Client_MessageReceived(SocketMessage arg)
        {
            if (arg.Author.Id == Client.CurrentUser.Id || arg.Author.IsBot) return;

            if (arg.Channel.Id == Channels.checkChannel)
            {
                var toChan = Client.GetChannel(Channels.toChannel) as ISocketMessageChannel;
                bool dr2 = false;
                foreach (var embed in arg.Embeds)
                {                    
                    dr2 = embed.Description.Contains("Dirt Rally 2.0");
                    _logger.LogDebug("Checking Embed for Dirt Rally 2.0");
                }
                if (dr2)
                {
                    _logger.LogInformation("Dirt Rally 2.0 Clip found and reposted.");
                    await toChan.SendMessageAsync(arg.Content);                    
                }
            }
            _logger.LogDebug(arg.Content);
            await Task.CompletedTask;            
        }

        private Task Client_Log(LogMessage arg)
        {
            switch (arg.Severity)
            {
                case LogSeverity.Critical:
                    _logger.LogCritical(arg.Message);
                    break;
                case LogSeverity.Debug:
                    _logger.LogDebug(arg.Message);
                    break;
                case LogSeverity.Error:
                    _logger.LogError(arg.Message);
                    break;
                case LogSeverity.Info:
                    _logger.LogInformation(arg.Message);
                    break;
                case LogSeverity.Verbose:
                    _logger.LogTrace(arg.Message);
                    break;
                case LogSeverity.Warning:
                    _logger.LogWarning(arg.Message);
                    break;
                default:
                    break;
            }
            return Task.CompletedTask;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Application started");
            await RunAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Application shutting down");
            return Task.CompletedTask;
        }
    }
}
