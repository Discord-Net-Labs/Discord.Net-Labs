using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the info class of an attribute based method for command type <see cref="ApplicationCommandType.User"/>
    /// </summary>
    public class UserCommandInfo : ContextCommandInfo
    {
        internal UserCommandInfo (Builders.ContextCommandBuilder builder, ModuleInfo module, InteractionService commandService)
            : base(builder, module, commandService) { }

        /// <inheritdoc/>
        public override async Task<IResult> ExecuteAsync (IInteractionCommandContext context, IServiceProvider services)
        {
            try
            {
                if (context.Interaction is not SocketUserCommand userCommand )
                    return ExecuteResult.FromError(InteractionCommandError.ParseFailed, $"Provided {nameof(IInteractionCommandContext)} does not belong to a User Command");

                var user = userCommand.Data.Member;
                object[] args = new object[1] { user };

                return await RunAsync(context, args, services).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return ExecuteResult.FromError(ex);
            }
        }

        /// <inheritdoc/>
        protected override string GetLogString (IInteractionCommandContext context)
        {
            if (context.Guild != null)
                return $"User Command: \"{Name}\" for {context.User} in {context.Guild}/{context.Channel}";
            else
                return $"User Command: \"{Name}\" for {context.User} in {context.Channel}";
        }
    }
}
