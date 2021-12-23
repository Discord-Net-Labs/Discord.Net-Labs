using System.Threading.Tasks;
using System.Linq;
using System;
using System.Reflection;

namespace Discord.Interactions
{
    public static class IDiscordInteractionExtentions
    {
        public static async Task RespondWithModalAsync(this IDiscordInteraction interaction, IModal modal,
            RequestOptions options = null) 
        {
            Type t = modal.GetType();
            var modalAttribute = t.GetCustomAttribute<ModalInteractionAttribute>();

            var builder = new ModalBuilder()
                .WithCustomId(modalAttribute?.CustomId ?? t.FullName)
                .WithTitle(modalAttribute?.Title ?? t.Name);

            t.GetProperties()
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