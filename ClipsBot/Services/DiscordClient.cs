using ClipsBot.Ignore;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Api.Interfaces;
using TwitchLib.Api.V5.Models.Clips;

namespace ClipsBot.Services
{
    public class DiscordClient : IHostedService
    {
        public DiscordSocketClient Client { get; private set; }

        private readonly ILogger<DiscordClient> _logger;
        private readonly ITwitchAPI _api;

        public DiscordClient(ILogger<DiscordClient> logger, ITwitchAPI api)
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
            var parse = linkParser.Matches(arg.Content);

            if (parse.Count == 0)
            {
                _logger.LogInformation("No link found");
                return;
            }

            var link = parse[0].Value;
            int pos = link.LastIndexOf("/") + 1;
            var slug = link[pos..];

            try
            {
                var clip = await _api.V5.Clips.GetClipAsync(slug);

                var toChan = Client.GetChannel(Channels.ToChannel) as ISocketMessageChannel;

                if (clip.Game.ToLower() == "dirt rally 2.0")
                {
                    var n = clip.Url.IndexOf('?');
                    var s = clip.Url.Substring(0, n != -1 ? n : clip.Url.Length);
                    await toChan.SendMessageAsync($"<{s}>", embed: SetupEmbed(clip,s).Build());
                    _logger.LogInformation("Dirt Rally 2.0 Clip found and reposted");
                }
                else
                {
                    _logger.LogInformation("Dirt Rally 2.0 Clip NOT found and ignored");
                }
                _logger.LogDebug($"{Globals.CurrentTime} DetectLOG   {arg.Content}");
            }
            catch { } // If fails ignore
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

        private EmbedBuilder SetupEmbed(Clip clip, string s)
        {
            EmbedBuilder eb = new EmbedBuilder()
            {
                Color = new Color(217, 104, 15),
                Title = "📎 " + clip.Broadcaster.DisplayName,
                Description = clip.Title,
                Url = clip.Broadcaster.ChannelUrl
            };
            eb.AddField(x =>
            {
                x.Name = $"Clipped By";
                x.Value = clip.Curator.DisplayName;
                x.IsInline = true;
            });
            eb.AddField(x =>
            {
                x.Name = $"Duration";
                x.Value = clip.Duration;
                x.IsInline = true;
            });
            eb.AddField(x =>
            {
                x.Name = $"Views";
                x.Value = clip.Views;
                x.IsInline = true;
            });
            eb.WithImageUrl(clip.Thumbnails.Medium);
            eb.WithThumbnailUrl(clip.Broadcaster.Logo);
            eb.WithFooter(x => { x.Text = s; });
            return eb;
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
