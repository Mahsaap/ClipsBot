using ClipsBot.Models.Configuration;
using Discord.Commands;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClipsBot.Preconditions
{
    class RequireID : PreconditionAttribute
    {
        private readonly DiscordOptions _discordOptions;

        public RequireID(IOptions<DiscordOptions> discordOptions)
        {
            _discordOptions = discordOptions.Value;
        }
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {

            var ids = new List<ulong> { };
            foreach (var id in _discordOptions.AdminId.Ids)
            {
                ids.Add(id);
            }
            if (ids.Contains(context.User.Id))
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }
            else
                return Task.FromResult(PreconditionResult.FromError("You do not have the required user ID"));
        }
    }
}