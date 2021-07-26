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
        /// <returns>
        /// A Task representing the asyncronous waiting operation with a <see cref="IDiscordInteraction"/> result,
        /// the result is null if the process timed out before receiving a valid Interaction.
        /// </returns>
        public static async Task<SocketInteraction> WaitForInteraction (BaseSocketClient client, TimeSpan timeout,
            Predicate<SocketInteraction> predicate)
        {
            SocketInteraction next = null;

            using (var handle = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                client.InteractionCreated += Next;
                await handle.WaitOneAsync(timeout).ConfigureAwait(false);
                handle.Close();
                client.InteractionCreated -= Next;
                return next;

                Task Next (SocketInteraction interaction)
                {
                    if (predicate(interaction))
                    {
                        next = interaction;
                        handle.Set();
                    }
                    return Task.CompletedTask;
                }
            }
        }

        /// <summary>
        /// Wait for an Interaction event for a given amount of time as an asynchronous opration
        /// </summary>
        /// <param name="context">Context the revieved Interations should be compared with</param>
        /// <param name="timeout">Timeout duration for this operation</param>
        /// <param name="fromSameUser">Wait for an Interaction event raised by the same <see cref="IUser"/></param>
        /// <param name="fromSameChannel">Wait for an Interaction event raised from the same <see cref="IChannel"/></param>
        /// <param name="withCustomId">Wait for a <see cref="IMessageComponent"/> Interaction with this Custom Id, should be left null if any
        /// Interaction should pass this criteria</param>
        /// <returns>
        /// A Task representing the asyncronous waiting operation with a <see cref="IDiscordInteraction"/> result,
        /// the result is null if the process timed out before receiving a valid Interaction.
        /// </returns>
        public static async Task<SocketInteraction> WaitForInteraction (ISlashCommandContext context, TimeSpan timeout,
            bool fromSameUser = true, bool fromSameChannel = true, string withCustomId = null)
        {
            Predicate<SocketInteraction> predicate = (interaction) =>
            {
                if (( !fromSameChannel || ( interaction.Channel.Id == context.Channel.Id ) ) && ( !fromSameUser || ( interaction.User.Id == context.User.Id ) ))
                {
                    if (withCustomId != null)
                    {
                        if (interaction is SocketMessageComponent messageInteraction && messageInteraction.Data.CustomId == withCustomId)
                            return true;
                    }
                    else
                        return true;
                }

                return false;
            };

            BaseSocketClient client;

            if (context is SocketSlashCommandContext socketCtx)
                client = socketCtx.Client;
            else if (context is ShardedSlashCommandContext shardedCtx)
                client = shardedCtx.Client;
            else
                throw new InvalidOperationException("Type of client provided with the context is not supported");

            return await WaitForInteraction(client, timeout, predicate);
        }

        internal static Task<bool> WaitOneAsync (this EventWaitHandle handle, int timeout = Timeout.Infinite)
        {
            if (handle == null)
                throw new ArgumentNullException("Wait handle");

            var tcs = new TaskCompletionSource<bool>();
            var rwh = ThreadPool.RegisterWaitForSingleObject(handle, (state, dur) =>
            {
                tcs.TrySetResult(true);
            }, null, timeout, true);

            var task = tcs.Task;
            _ = task.ContinueWith(x => rwh.Unregister(null));
            return task;
        }

        internal static Task<bool> WaitOneAsync (this EventWaitHandle handle, TimeSpan timeout) =>
            WaitOneAsync(handle, (int)timeout.TotalMilliseconds);
    }
}
