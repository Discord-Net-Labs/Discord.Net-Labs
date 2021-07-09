using Discord.WebSocket;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    /// <summary>
    /// 
    /// </summary>
    public static class InteractionUtility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="timeout"></param>
        /// <param name="fromSameUser"></param>
        /// <param name="fromSameChannel"></param>
        /// <returns></returns>
        public static async Task<IDiscordInteraction> WaitForInteraction (ISlashCommandContext context, TimeSpan timeout,
            bool fromSameUser = true, bool fromSameChannel = true, string withCustomId = null)
        {
            SocketInteraction next = null;

            using (var handle = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                DiscordSocketClient client;

                if (context.Client is DiscordSocketClient socketClient)
                    client = socketClient;
                else if (context.Client is DiscordShardedClient shardedClient)
                    client = shardedClient.GetShardFor(context.Guild);
                else
                    throw new InvalidOperationException("Type of client provided with the context is not supported");

                client.InteractionRecieved += Next;
                await handle.WaitOneAsync(timeout).ConfigureAwait(false);
                handle.Close();
                client.InteractionRecieved -= Next;
                return next;

                Task Next (SocketInteraction interaction)
                {
                    if (( !fromSameChannel || ( interaction.Channel == context.Channel ) ) && ( !fromSameUser || ( interaction.User == context.User ) ))
                    {
                        if(!string.IsNullOrEmpty(withCustomId) && interaction is SocketMessageInteraction messageInteraction)
                        {
                            if(messageInteraction.CustomId == withCustomId)
                            {
                                next = interaction;
                                handle.Set();
                            }
                        }
                        else
                        {
                            next = interaction;
                            handle.Set();
                        }
                    }
                    return Task.CompletedTask;
                }
            }
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
