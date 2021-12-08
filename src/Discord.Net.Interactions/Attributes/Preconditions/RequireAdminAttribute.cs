using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    public class RequireAdminAttribute : PreconditionAttribute
    {
        /// <summary>
        ///     Gets or sets the error message if the precondition
        ///     fails due to being run outside of a Guild channel.
        /// </summary>
        public string NotAGuildErrorMessage { get; set; }

        public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            if (context.User is not IGuildUser guildUser)
                return Task.FromResult(PreconditionResult.FromError(NotAGuildErrorMessage ?? "Command must be used in a guild channel."));

            if (guildUser.GuildPermissions.Administrator)
                return Task.FromResult(PreconditionResult.FromSuccess());
            else
                return Task.FromResult(PreconditionResult.FromError(ErrorMessage ?? $"User requires Administrator permission."));
        }
    }
}
