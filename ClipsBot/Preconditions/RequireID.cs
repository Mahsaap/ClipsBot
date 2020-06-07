using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClipsBot.Preconditions
{
    class RequireID : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var ids = new List<ulong> { 290501197255671809, 88798728948809728 };
            foreach (var id in ids.ToArray())
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