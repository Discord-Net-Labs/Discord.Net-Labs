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
            BaseSocketClient client;
            SocketInteraction next = null;

            if (context is SocketSlashCommandContext socketCtx)
                client = socketCtx.Client;
            else if (context is ShardedSlashCommandContext shardedCtx)
                client = shardedCtx.Client;
            else
                throw new InvalidOperationException("Type of client provided with the context is not supported");

            using (var handle = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                client.InteractionCreated += Next;
                await handle.WaitOneAsync(timeout).ConfigureAwait(false);
                handle.Close();
                client.InteractionCreated -= Next;
                return next;

                Task Next (SocketInteraction interaction)
                {
                    if (( !fromSameChannel || ( interaction.Channel == context.Channel ) ) && ( !fromSameUser || ( interaction.User == context.User ) ))
                    {
                        if(!string.IsNullOrEmpty(withCustomId))
                        {
                            if(interaction is SocketMessageComponent messageInteraction && messageInteraction.Data.CustomId == withCustomId)
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
