using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Represents the information class of an attribute based method for command type <see cref="ApplicationCommandType.User"/>
    /// </summary>
    public class UserCommandInfo : ContextCommandInfo
    {
        internal UserCommandInfo (Builders.ContextCommandBuilder builder, ModuleInfo module, SlashCommandService commandService)
            : base(builder, module, commandService) { }

        /// <inheritdoc/>
        public override async Task<IResult> ExecuteAsync (ISlashCommandContext context, IServiceProvider services)
        {
            if (!( context.Interaction is SocketUserCommand userCommand ))
                return ExecuteResult.FromError(SlashCommandError.ParseFailed, $"Provided {nameof(ISlashCommandContext)} does not belong to a User Command");

            var user = userCommand.Data.Member;
            object[] args = new object[1] { user };

            return await RunAsync(context, args, services).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override string GetLogString (ISlashCommandContext context)
        {
            if (context.Guild != null)
                return $"User Command: \"{Name}\" for {context.User} in {context.Guild}/{context.Channel}";
            else
                return $"User Command: \"{Name}\" for {context.User} in {context.Channel}";
        }
    }
}
