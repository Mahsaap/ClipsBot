using ClipsBot.Ignore;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Api;

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

            _logger.LogInformation("Client Created!");

            Client.Log += Client_Log;
            Client.MessageReceived += Client_MessageReceived;

            await Client.LoginAsync(TokenType.Bot, DiscordToken.Token);
            await Client.StartAsync();

            _logger.LogInformation("Discord Client Started!");

            await Task.Delay(-1);
        }

        private async Task Client_MessageReceived(SocketMessage arg)
        {
            if (string.IsNullOrEmpty(arg.Content)) return;
            if (arg.Author.Id == Client.CurrentUser.Id || arg.Author.IsBot) return;
            if (arg.Channel.Id != Channels.CheckChannel) return;

            var clip = await _api.V5.Clips.GetClipAsync("");

            /*
             * https://clips.twitch.tv/CredulousAssiduousWombatMrDestructoid
             * https://www.twitch.tv/juniorrallychampionship/clip/BoredPoorCardKappaClaus
             */
            if (clip.Game != "") return; //Check for DR2


            var toChan = Client.GetChannel(Channels.ToChannel) as ISocketMessageChannel;
            var msg = arg.Channel.GetCachedMessage(arg.Id);

            foreach (var embed in msg.Embeds)
            {
                //Console.WriteLine($"{Globals.CurrentTime} Detect      Checking Embed for Dirt Rally 2.0");
                bool dr2 = embed.Description.ToLower().Contains("dirt rally 2.0");
                if (dr2)
                {
                    _logger.LogInformation($"{Globals.CurrentTime} Detect      Dirt Rally 2.0 Clip found and reposted");
                    await toChan.SendMessageAsync(embed.Url);
                }
                else
                {
                    _logger.LogInformation($"{Globals.CurrentTime} Detect      Dirt Rally 2.0 Clip NOT found and ignored");
                }
            }

            _logger.LogDebug($"{Globals.CurrentTime} DetectLOG   {arg.Content}");
            //await Task.CompletedTask;          
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
