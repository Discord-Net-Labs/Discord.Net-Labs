using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    public class UserCommandInfo : ContextCommandInfo
    {
        internal UserCommandInfo (Builders.ContextCommandBuilder builder, ModuleInfo module, SlashCommandService commandService)
            : base(builder, module, commandService) { }

        public override async Task<IResult> ExecuteAsync (ISlashCommandContext context, IServiceProvider services)
        {
            if (!( context.Interaction is SocketUserCommand userCommand ))
                return ExecuteResult.FromError(SlashCommandError.ParseFailed, $"Provided {nameof(ISlashCommandContext)} does not belong to a User Command");

            services = services ?? EmptyServiceProvider.Instance;

            try
            {
                var user = userCommand.Data.Member;

                object[] args = new object[1] { user };

                if (CommandService._runAsync)
                {
                    _ = Task.Run(async ( ) =>
                    {
                        await ExecuteInternalAsync(context, args, services).ConfigureAwait(false);
                    });
                }
                else
                    return await ExecuteInternalAsync(context, args, services).ConfigureAwait(false);

                return ExecuteResult.FromSuccess();
            }
            catch (Exception ex)
            {
                return ExecuteResult.FromError(ex);
            }
        }

        protected override string GetLogString (ISlashCommandContext context)
        {
            if (context.Guild != null)
                return $"User Command: \"{Name}\" for {context.User} in {context.Guild}/{context.Channel}";
            else
                return $"User Command: \"{Name}\" for {context.User} in {context.Channel}";
        }
    }
}
