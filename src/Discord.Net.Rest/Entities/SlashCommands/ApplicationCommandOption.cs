using System.Collections.Generic;
using System.Linq;
using Model = Discord.API.ApplicationCommandOption;

namespace Discord
{
    /// <inheritdoc cref="IApplicationCommandOption"/>
    public class ApplicationCommandOption : IApplicationCommandOption
    {
        /// <inheritdoc/>
        public const int MaxOptionDepth = 4;
        /// <inheritdoc/>
        public ApplicationCommandOptionType OptionType { get; }
        /// <inheritdoc/>
        public string Name { get; }
        /// <inheritdoc/>
        public string Description { get; }
        /// <inheritdoc/>
        public bool IsRequired { get; }
        /// <inheritdoc cref="IApplicationCommandOption.Choices"/>
        public IReadOnlyDictionary<string, object> Choices { get; }
        /// <inheritdoc cref="IApplicationCommandOption.Options"/>
        public IReadOnlyList<ApplicationCommandOption> Options { get; }

        /// <inheritdoc/>
        IEnumerable<KeyValuePair<string, object>> IApplicationCommandOption.Choices => Choices;
        /// <inheritdoc/>
        IEnumerable<IApplicationCommandOption> IApplicationCommandOption.Options => Options;

        private ApplicationCommandOption ( Model model, IEnumerable<ApplicationCommandOption> options )
        {
            Name = model.Name;
            Description = model.Description;
            OptionType = model.Type;
            if (model.Required.IsSpecified)
                IsRequired = model.Required.Value;
            if (model.Choices.IsSpecified)
                Choices = model.Choices.Value.ToDictionary(x => x.Name, x => x.Value);

            Options = options?.ToList();  
        }

        internal static ApplicationCommandOption Create(Model model, int ttl)
        {
            if (ttl <= 0)
                throw new System.Exception("Recursive model creation is dumped, children count is higher than expected");
            else if (!model.Options.IsSpecified || model.Options.Value == null)
                return new ApplicationCommandOption(model, null);

            var subOptions = model.Options.Value.Select(x => ApplicationCommandOption.Create(x, --ttl));
            return new ApplicationCommandOption(model, subOptions);
        }
    }
}
