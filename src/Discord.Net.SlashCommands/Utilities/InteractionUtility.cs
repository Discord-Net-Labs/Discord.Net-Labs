using Discord.WebSocket;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    public static class InteractionUtility
    {
        /// <summary>
        /// Wait for an Interaction event for a given amount of time as an asynchronous opration
        /// </summary>
        /// <param name="client">Client that should be listened to for the <see cref="BaseSocketClient.InteractionCreated"/> event</param>
        /// <param name="timeout">Timeout duration for this operation</param>
        /// <param name="predicate">Delegate for cheking whether an Interaction meets the requirements</param>
        /// <param name="cancellationToken">Token for canceling the wait operation</param>
        /// <returns>
        /// A Task representing the asyncronous waiting operation with a <see cref="IDiscordInteraction"/> result,
        /// the result is null if the process timed out before receiving a valid Interaction.
        /// </returns>
        public static async Task<SocketInteraction> WaitForInteraction (BaseSocketClient client, TimeSpan timeout,
            Predicate<SocketInteraction> predicate, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<SocketInteraction>();

            var waitCancelSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            Task wait = Task.Delay(timeout, waitCancelSource.Token)
                .ContinueWith((t) =>
                {
                    if (!t.IsCanceled)
                        tcs.SetResult(null);
                });

            cancellationToken.Register(( ) => tcs.SetCanceled());

            client.InteractionCreated += HandleInteraction;
            var result = await tcs.Task.ConfigureAwait(false);
            client.InteractionCreated -= HandleInteraction;

            return result;

            Task HandleInteraction (SocketInteraction interaction)
            {
                if (predicate(interaction))
                {
                    waitCancelSource.Cancel();
                    tcs.SetResult(interaction);
                }

                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Wait for an Message Component Interaction event for a given amount of time as an asynchronous opration
        /// </summary>
        /// <param name="client">Client that should be listened to for the <see cref="BaseSocketClient.InteractionCreated"/> event</param>
        /// <param name="ctx">Command context that will be used to determine the target user and the target channel</param>
        /// <param name="timeout">Timeout duration for this operation</param>
        /// <param name="sameUser">Wait for an interaction that is from the same user as in the <paramref name="ctx"/></param>
        /// <param name="sameChannel">Wait for an interaction that is from the same channel as in the <paramref name="ctx"/></param>
        /// <param name="cancellationToken">Token for canceling the wait operation</param>
        /// <returns>
        /// A Task representing the asyncronous waiting operation with a <see cref="IDiscordInteraction"/> result,
        /// the result is null if the process timed out before receiving a valid Interaction.
        /// </returns>
        public static async Task<SocketInteraction> WaitForMessageComponent (BaseSocketClient client, ISlashCommandContext ctx, TimeSpan timeout, bool sameUser = true,
            bool sameChannel = true, CancellationToken cancellationToken = default)
        {
            Predicate<SocketInteraction> predicate = (interaction) => CheckMessageComponent(ctx, interaction, null, sameUser, sameChannel);

            return await WaitForInteraction(client, timeout, predicate, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Create a confirmation dialog and wait for user input asynchronously
        /// </summary>
        /// <param name="client">Client that should be listened to for the <see cref="BaseSocketClient.InteractionCreated"/> event</param>
        /// <param name="ctx">Command context that will be used to determine the target user and the target channel</param>
        /// <param name="timeout">Timeout duration of this operation</param>
        /// <param name="message">Optional custom prompt message</param>
        /// <param name="cancellationToken">Token for canceling the wait operation</param>
        /// <returns>
        /// A Task representing the asyncronous waiting operation with a <see cref="bool"/> result,
        /// the result is <see cref="false"/> if the user declined the prompt or didnt answer in time, <see cref="true"/> if the user confirmed the prompt
        /// </returns>
        public static async Task<bool> Confirm (BaseSocketClient client, ISlashCommandContext ctx, TimeSpan timeout, string message = null,
            CancellationToken cancellationToken = default)
        {
            var guid = Guid.NewGuid();

            message = message ?? "Would you like to continue?";
            var confirmId = $"{guid}:confirm";
            var declineId = $"{guid}:decline";

            var component = new ComponentBuilder()
                .WithButton("Confirm", confirmId, ButtonStyle.Success)
                .WithButton("Cancel", declineId, ButtonStyle.Danger)
                .Build();

            var dialog = await ctx.Channel.SendMessageAsync(message, component: component).ConfigureAwait(false);

            var response = await WaitForInteraction(client, timeout, (interaction) =>
            {
                return CheckMessageComponent(ctx, interaction, confirmId) || CheckMessageComponent(ctx, interaction, declineId);
            }, cancellationToken).ConfigureAwait(false) as SocketMessageComponent;

            await dialog.DeleteAsync().ConfigureAwait(false);

            if (response != null && response.Data.CustomId == confirmId)
                return true;
            else
                return false;
        }

        private static bool CheckMessageComponent (ISlashCommandContext ctx, SocketInteraction interaction, string customId = null,
            bool sameUser = true, bool sameChannel = true)
        {
            return interaction.Type == InteractionType.MessageComponent &&
                ( !sameUser || ctx.User.Id == interaction.User.Id ) &&
                ( !sameChannel || ctx.Channel.Id == interaction.Channel.Id ) &&
                ( customId == null || ( interaction as SocketMessageComponent )?.Data.CustomId == customId );
        }
    }
}
