using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Represents the information class of an attribute based method for command type <see cref="ApplicationCommandType.Message"/>
    /// </summary>
    public class MessageCommandInfo : ContextCommandInfo
    {
        internal MessageCommandInfo (Builders.ContextCommandBuilder builder, ModuleInfo module, SlashCommandService commandService)
            : base(builder, module, commandService) { }

        /// <inheritdoc/>
        public override async Task<IResult> ExecuteAsync (ISlashCommandContext context, IServiceProvider services)
        {
            if (!( context.Interaction is SocketMessageCommand messageCommand ))
                return ExecuteResult.FromError(SlashCommandError.ParseFailed, $"Provided {nameof(ISlashCommandContext)} does not belong to a Message Command");

            var message = messageCommand.Data.Message;
            object[] args = new object[1] { message };

            return await RunAsync(context, args, services).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override string GetLogString (ISlashCommandContext context)
        {
            if (context.Guild != null)
                return $"Message Command: \"{Name}\" for {context.User} in {context.Guild}/{context.Channel}";
            else
                return $"Message Command: \"{Name}\" for {context.User} in {context.Channel}";
        }
    }
}
