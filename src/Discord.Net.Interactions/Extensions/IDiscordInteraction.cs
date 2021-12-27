using System.Threading.Tasks;
using System.Linq;
using System;
using System.Reflection;

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
        public static async Task RespondWithModalAsync(this IDiscordInteraction interaction, IModal modal, RequestOptions options = null) 
        {
            var builder = new ModalBuilder()
                .WithCustomId(modal.CustomId)
                .WithTitle(modal.Title);

            modal
                .GetType()
                .GetProperties()
                .Where(x => x.PropertyType == typeof(string))
                .Select(x => x.GetCustomAttribute<ModalTextInputAttribute>())
                .Where(x => x != null)
                .ToList()
                .ForEach(input => builder.AddTextInput(input.Label, input.CustomId, input.Style, input.Placeholder,
                    input.MinLength, input.MaxLength, input.Required, input.Value));

            await interaction.RespondWithModalAsync(builder.Build(), options);
        }
    }
}