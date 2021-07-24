using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.ApplicationCommandOption;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a Rest-based implementation of <see cref="IApplicationCommandOption"/>.
    /// </summary>
    public class ApplicationCommandOption : IApplicationCommandOption
    {
        /// <inheritdoc/>
        public ApplicationCommandOptionType Type { get; private set; }

        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <inheritdoc/>
        public string Description { get; private set; }

        /// <inheritdoc/>
        public bool? Default { get; private set; }

        /// <inheritdoc/>
        public bool? Required { get; private set; }

        /// <summary>
        ///     A collection of <see cref="ApplicationCommandChoice"/>'s for this command.
        /// </summary>
        public IReadOnlyCollection<ApplicationCommandChoice> Choices { get; private set; }

        /// <summary>
        ///     A collection of <see cref="ApplicationCommandOption"/>'s for this command.
        /// </summary>
        public IReadOnlyCollection<ApplicationCommandOption> Options { get; private set; }

        internal ApplicationCommandOption ( ) { }

        internal static ApplicationCommandOption Create (Model model)
        {
            var options = new ApplicationCommandOption();
            options.Update(model);
            return options;
        }

        internal void Update (Model model)
        {
            this.Type = model.Type;
            this.Name = model.Name;
            this.Description = model.Description;

            if (model.Default.IsSpecified)
                this.Default = model.Default.Value;

            if (model.Required.IsSpecified)
                this.Required = model.Required.Value;

            this.Options = model.Options.IsSpecified
                ? model.Options.Value.Select(x => Create(x)).ToImmutableArray()
                : null;

            this.Choices = model.Choices.IsSpecified
                ? model.Choices.Value.Select(x => new ApplicationCommandChoice(x)).ToImmutableArray()
                : null;
        }

        IReadOnlyCollection<IApplicationCommandOption> IApplicationCommandOption.Options => Options;
        IReadOnlyCollection<IApplicationCommandOptionChoice> IApplicationCommandOption.Choices => Choices;
    }
}
