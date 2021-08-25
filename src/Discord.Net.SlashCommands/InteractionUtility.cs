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
        /// <param name="predicate">Delegate for cheking wheter an Interaction meets the requirements</param>
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
        /// Wait for an Interaction event for a given amount of time as an asynchronous opration
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="timeout">Timeout duration for this operation</param>
        /// <param name="sameUser"></param>
        /// <param name="sameChannel"></param>
        /// <param name="cancellationToken">Token for canceling the wait operation</param>
        /// <returns>
        /// A Task representing the asyncronous waiting operation with a <see cref="IDiscordInteraction"/> result,
        /// the result is null if the process timed out before receiving a valid Interaction.
        /// </returns>
        public static async Task<SocketInteraction> WaitForMessageComponent (ISlashCommandContext ctx, TimeSpan timeout, bool sameUser = true,
            bool sameChannel = true, CancellationToken cancellationToken = default)
        {
            if(!(ctx.Client is BaseSocketClient baseSocketClient))
                throw new InvalidOperationException("Type of client provided with the context is not supported");

            Predicate<SocketInteraction> predicate = (interaction) => CheckMessageComponent(ctx, interaction, null, false, sameUser, sameChannel);

            return await WaitForInteraction(baseSocketClient, timeout, predicate, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Wait for an Interaction event for a given amount of time as an asynchronous opration
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="timeout">Timeout duration for this operation</param>
        /// <param name="customId">Only accept interactions from a <see cref="MessageComponent"/> with this custom id</param>
        /// <param name="sameUser"></param>
        /// <param name="sameChannel"></param>
        /// <param name="cancellationToken">Token for canceling the wait operation</param>
        /// <returns>
        /// A Task representing the asyncronous waiting operation with a <see cref="IDiscordInteraction"/> result,
        /// the result is null if the process timed out before receiving a valid Interaction.
        /// </returns>
        public static async Task<SocketInteraction> WaitForMessageComponent (ISlashCommandContext ctx, TimeSpan timeout, string customId, bool sameUser = true,
            bool sameChannel = true, CancellationToken cancellationToken = default)
        {
            if (!( ctx.Client is BaseSocketClient baseSocketClient ))
                throw new InvalidOperationException("Type of client provided with the context is not supported");

            Predicate<SocketInteraction> predicate = (interaction) => CheckMessageComponent(ctx, interaction, customId, true, sameUser, sameChannel);

            return await WaitForInteraction(baseSocketClient, timeout, predicate, cancellationToken).ConfigureAwait(false);
        }

        private static bool CheckMessageComponent(ISlashCommandContext ctx, SocketInteraction interaction, string customId = null, bool checkId = true,
            bool sameUser = true, bool sameChannel = true)
        {
            if (ctx.Interaction.Type != InteractionType.MessageComponent)
                return false;
            if (sameUser && ctx.User.Id != interaction.User.Id)
                return false;
            if (sameChannel && ctx.Channel.Id != interaction.Channel.Id)
                return false;
            if (( ctx.Interaction as SocketMessageComponent ).Data.CustomId != customId)
                return false;

            return true;
        }
    }
}
