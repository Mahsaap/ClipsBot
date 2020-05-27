using ClipsBot.Ignore;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.V5.Models.Clips;

namespace ClipsBot.Services
{
    public class DiscordClient : IHostedService
    {
        public DiscordSocketClient Client { get; private set; }

        private readonly ILogger<DiscordClient> _logger;
        private readonly TwitchAPI _api;

        public DiscordClient(ILogger<DiscordClient> logger, TwitchAPI api)
        {
            _logger = logger;
            _api = api;
        }

        public async Task RunAsync()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig { LogLevel = LogSeverity.Debug, MessageCacheSize = 100 });

            _logger.LogInformation($"{Globals.CurrentTime} Client Created!");

            Client.Log += Client_Log;
            Client.MessageReceived += Client_MessageReceived;

            await Client.LoginAsync(TokenType.Bot, DiscordToken.Token);
            await Client.StartAsync();

            _logger.LogInformation($"{Globals.CurrentTime} Discord Client Started!");

            await Task.Delay(-1);
        }

        private async Task Client_MessageReceived(SocketMessage arg)
        {
            if (string.IsNullOrEmpty(arg.Content)) return;
            if (arg.Author.Id == Client.CurrentUser.Id || arg.Author.IsBot) return;
            if (arg.Channel.Id != Channels.CheckChannel) return;

            var linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var link = linkParser.Matches(arg.Content)[0].Value;
            int pos = link.LastIndexOf("/") + 1;
            var slug = link[pos..];
            try
            {
                var clip = await _api.V5.Clips.GetClipAsync(slug);                

                var toChan = Client.GetChannel(Channels.ToChannel) as ISocketMessageChannel;

                if (clip.Game.ToLower() == "dirt rally 2.0")
                {
                    _logger.LogInformation("Dirt Rally 2.0 Clip found and reposted");
                    await toChan.SendMessageAsync(clip.Url);
                }
                else
                {
                    _logger.LogInformation("Dirt Rally 2.0 Clip NOT found and ignored");
                }
                _logger.LogDebug($"{Globals.CurrentTime} DetectLOG   {arg.Content}");      
            }
            catch { }

            /*
             * https://clips.twitch.tv/CredulousAssiduousWombatMrDestructoid
             * https://www.twitch.tv/juniorrallychampionship/clip/BoredPoorCardKappaClaus
             */
            
        }

        private Task Client_Log(LogMessage arg)
        {
            var layout = $"{Globals.CurrentTime} " + arg.Message;
            switch (arg.Severity)
            {
                case LogSeverity.Critical:
                    _logger.LogCritical(layout);
                    break;
                case LogSeverity.Debug:
                    _logger.LogDebug(layout);
                    break;
                case LogSeverity.Error:
                    _logger.LogError(layout);
                    break;
                case LogSeverity.Info:
                    _logger.LogInformation(layout);
                    break;
                case LogSeverity.Verbose:
                    _logger.LogTrace(layout);
                    break;
                case LogSeverity.Warning:
                    _logger.LogWarning(layout);
                    break;
                default:
                    break;
            }
            return Task.CompletedTask;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{Globals.CurrentTime} Application started");
            await RunAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning($"{Globals.CurrentTime} Application shutting down");
            return Task.CompletedTask;
        }
    }
}
