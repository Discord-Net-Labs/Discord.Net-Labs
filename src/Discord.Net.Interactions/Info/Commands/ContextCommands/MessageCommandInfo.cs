using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the info class of an attribute based method for command type <see cref="ApplicationCommandType.Message"/>
    /// </summary>
    public class MessageCommandInfo : ContextCommandInfo
    {
        internal MessageCommandInfo (Builders.ContextCommandBuilder builder, ModuleInfo module, InteractionService commandService)
            : base(builder, module, commandService) { }

        /// <inheritdoc/>
        public override async Task<IResult> ExecuteAsync (IInteractionContext context, IServiceProvider services)
        {
            try
            {
                object[] args;

                switch (context.Interaction)
                {
                    case RestMessageCommand restMessageCommand:
                        args = new object[1] { restMessageCommand.Data.Message };
                        break;
                    case SocketMessageCommand socketMessageCommand:
                        args = new object[1] { socketMessageCommand.Data.Message };
                        break;
                    default:
                        return ExecuteResult.FromError(InteractionCommandError.ParseFailed, $"Provided {nameof(IInteractionContext)} doesn't belong to a Message Command Interation");
                }

                return await RunAsync(context, args, services).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return ExecuteResult.FromError(ex);
            }
        }

        /// <inheritdoc/>
        protected override string GetLogString (IInteractionContext context)
        {
            if (context.Guild != null)
                return $"Message Command: \"{base.ToString()}\" for {context.User} in {context.Guild}/{context.Channel}";
            else
                return $"Message Command: \"{base.ToString()}\" for {context.User} in {context.Channel}";
        }
    }
}
