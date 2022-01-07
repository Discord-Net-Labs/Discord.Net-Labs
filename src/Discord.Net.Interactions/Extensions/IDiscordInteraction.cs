using System.Threading.Tasks;

namespace Discord.Interactions
{
    public static class IDiscordInteractionExtentions
    {
        /// <summary>
        ///     Respond to an interaction with a <see cref="IModal"/>.
        /// </summary>
        /// <param name="interaction">The interaction to respond to.</param>
        /// <param name="modal">The modal to respond with.</param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        /// <returns>A task that represents the asynchronous operation of responding to the interaction.</returns>
        public static async Task RespondWithModalAsync<T>(this IDiscordInteraction interaction, InteractionService interactionService, string customId, RequestOptions options = null)
            where T : class, IModal
        {
            var modalInfo = interactionService.GetModalInfo(typeof(T));

            var builder = new ModalBuilder()
                .WithCustomId(customId)
                .WithTitle(modalInfo.Title);

            foreach (var textInput in modalInfo.TextComponents)
                builder.AddTextInput(textInput.Label, textInput.CustomId, textInput.Style, textInput.Placeholder, textInput.MinLength, textInput.MaxLength, textInput.IsRequired, textInput.Value);

            await interaction.RespondWithModalAsync(builder.Build(), options);
        }
    }
}
