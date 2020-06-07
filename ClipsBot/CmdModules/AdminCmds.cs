using ClipsBot.Preconditions;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace ClipsBot.CmdModules
{
    [Name("Admin")]
    [RequireContext(ContextType.DM)]
    [RequireID]
    public class AdminCmds : ModuleBase<SocketCommandContext>
    {
        private readonly IHostApplicationLifetime _host;

        public AdminCmds(IHostApplicationLifetime host)
        {
            _host = host;
        }

        [Command("version")]
        public async Task VersionAsync() => await ReplyAsync($"V{Version.Major}.{Version.Minor}");

        //End application - ConsoleApp
        [Command("shutdown"), Summary("Shuts down the bot.")]
        public async Task ShutdownBotAsync()
        {
            await Context.Client.SetStatusAsync(UserStatus.Invisible);
            await Task.Delay(500);
            await ReplyAsync("**Shutting Down!**");
            _host.StopApplication();
        }
    }
}
