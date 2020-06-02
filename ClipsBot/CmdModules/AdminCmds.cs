using ClipsBot.Preconditions;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ClipsBot.CmdModules
{
    [Name("Admin")]
    [RequireContext(ContextType.DM)]
    [RequireID]
    public class AdminCmds : ModuleBase<SocketCommandContext>
    {
        [Command("version")]
        public async Task VersionAsync()
        {
            await ReplyAsync($"V{Version.Major}.{Version.Minor}");
        }

        //End application - ConsoleApp
        [Command("shutdown"), Summary("Shuts down the bot.")]
        public async Task ShutdownBotAsync()
        {
            await Context.Client.SetStatusAsync(UserStatus.Invisible);
            await Task.Delay(500);
            await ReplyAsync("**Shutting Down!**");
            Environment.Exit(1);
        }

        [Command("setplaying", RunMode = RunMode.Async)]
        public async Task SetPlayingAsync([Remainder]string entry)
        {
            await Context.Client.SetGameAsync(entry);
        }
    }
}
