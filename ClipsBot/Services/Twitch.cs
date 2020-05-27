using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TwitchLib.Api;
using TwitchLib.Api.Core.Exceptions;
using TwitchLib.Api.V5.Models.Channels;
using TwitchLib.Api.V5.Models.Teams;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;
using System.Threading;
using System.Threading.Tasks;
using ClipsBot.Ignore;

namespace ClipsBot.Services
{
    public class Twitch : IHostedService
    {
        public TwitchAPI API { get; private set; }

        private readonly ILogger<Twitch> _logger;

        public Twitch(ILogger<Twitch> logger)
        {
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Application started");
            await RunAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Application shutting down");
            await Task.CompletedTask;
        }

        private async Task RunAsync()
        {
            API = new TwitchAPI();

            API.Settings.ClientId = TwitchCreds.ClientID;
            API.Settings.Secret = TwitchCreds.Secret;

            _logger.LogInformation("Twitch API Initiated");
            await Task.CompletedTask;
        }
    }
}
