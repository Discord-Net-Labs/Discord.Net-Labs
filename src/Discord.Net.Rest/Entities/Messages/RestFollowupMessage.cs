using System;
using System.Threading.Tasks;
using Model = Discord.API.Message;


namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based follow up message sent by a bot responding to a slash command.
    /// </summary>
    public class RestFollowupMessage : RestUserMessage
    {
        /// <summary>
        /// The interaction this message belongs to
        /// </summary>
        public IDiscordInteraction Interaction { get; }

        internal RestFollowupMessage (BaseDiscordClient discord, ulong id, IUser author, IDiscordInteraction interaction, IMessageChannel channel)
            : base(discord, id, channel, author, MessageSource.Bot)
        {
            Interaction = interaction;
        }

        internal static RestFollowupMessage Create (BaseDiscordClient discord, Model model, IDiscordInteraction interaction, IMessageChannel channel)
        {
            var entity = new RestFollowupMessage(discord, model.Id, model.Author.IsSpecified ? RestUser.Create(discord, model.Author.Value) : discord.CurrentUser, interaction, channel);
            entity.Update(model);
            return entity;
        }

        internal new void Update (Model model)
        {
            base.Update(model);
        }

        /// <summary>
        ///     Deletes this object and all of it's childern.
        /// </summary>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        public async Task DeleteAsync ( )
            => await InteractionHelper.DeleteFollowupMessage(Discord, Interaction, Id, null).ConfigureAwait(false);

        /// <summary>
        ///     Modifies this interaction followup message.
        /// </summary>
        /// <remarks>
        ///     This method modifies this message with the specified properties. To see an example of this
        ///     method and what properties are available, please refer to <see cref="MessageProperties"/>.
        /// </remarks>
        /// <example>
        ///     <para>The following example replaces the content of the message with <c>Hello World!</c>.</para>
        ///     <code language="cs">
        ///     await msg.ModifyAsync(x =&gt; x.Content = "Hello World!");
        ///     </code>
        /// </example>
        /// <param name="func">A delegate containing the properties to modify the message with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation.
        /// </returns>
        /// <exception cref="InvalidOperationException">The token used to modify/delete this message expired.</exception>
        /// /// <exception cref="Discord.Net.HttpException">Somthing went wrong during the request.</exception>
        public new async Task ModifyAsync (Action<MessageProperties> func, RequestOptions options = null)
        {
            if (!Interaction.IsValidToken)
                throw new InvalidOperationException("The token of this message has expired!");

            try
            {
                var props = new MessageProperties();
                func(props);
                var model = await MessageHelper.ModifyFollowupMessage(Discord, Interaction, this, func, options).ConfigureAwait(false);
                Update(model);
            }
            catch (Discord.Net.HttpException x)
            {
                if (x.HttpCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new InvalidOperationException("The token of this message has expired!", x);
                }

                throw;
            }
        }
    }
}
