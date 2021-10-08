using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Discord
{
    /// <summary>
    ///     A class used to build slash commands.
    /// </summary>
    public class SlashCommandBuilder
    {
        /// <summary>
        ///     Returns the maximum length a commands name allowed by Discord
        /// </summary>
        public const int MaxNameLength = 32;
        /// <summary>
        ///     Returns the maximum length of a commands description allowed by Discord.
        /// </summary>
        public const int MaxDescriptionLength = 100;
        /// <summary>
        ///     Returns the maximum count of command options allowed by Discord
        /// </summary>
        public const int MaxOptionsCount = 25;

        /// <summary>
        ///     The name of this slash command.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                Preconditions.NotNullOrEmpty(value, nameof(value));
                Preconditions.AtLeast(value.Length, 1, nameof(value));
                Preconditions.AtMost(value.Length, MaxNameLength, nameof(value));

                // Discord updated the docs, this regex prevents special characters like @!$%(... etc,
                // https://discord.com/developers/docs/interactions/slash-commands#applicationcommand
                if (!Regex.IsMatch(value, @"^[\w-]{1,32}$"))
                    throw new ArgumentException("Command name cannot contain any special characters or whitespaces!", nameof(value));

                _name = value;
            }
        }

        /// <summary>
        ///    A 1-100 length description of this slash command.
        ///    The description is not allowed to be null.
        /// </summary>
        public string Description
        {
            get => _description;
            set
            {
                Preconditions.NotNullOrEmpty(value, nameof(Description));
                Preconditions.AtLeast(value.Length, 1, nameof(Description));
                Preconditions.AtMost(value.Length, MaxDescriptionLength, nameof(Description));

                _description = value;
            }
        }

        /// <summary>
        ///     Gets or sets the options for this command.
        /// </summary>
        public List<SlashCommandOptionBuilder> Options
        {
            get => _options;
            set
            {
                Preconditions.AtMost(value?.Count ?? 0, MaxOptionsCount, nameof(value));
                _options = value;
            }
        }

        /// <summary>
        ///     Whether the command is enabled by default when the app is added to a guild
        /// </summary>
        public bool DefaultPermission { get; set; } = true;

        private string _name;
        private string _description;
        private List<SlashCommandOptionBuilder> _options;

        /// <summary>
        ///     Build the current builder into a <see cref="SlashCommandProperties"/> class.
        /// </summary>
        /// <returns>A <see cref="SlashCommandProperties"/> that can be used to create slash commands over rest.</returns>
        public SlashCommandProperties Build()
        {
            var props = new SlashCommandProperties
            {
                Name = Name,
                Description = Description,
                DefaultPermission = DefaultPermission
            };

            if (Options != null && Options.Any())
            {
                var options = new List<ApplicationCommandOptionProperties>();

                Options.ForEach(x => options.Add(x.Build()));

                props.Options = options;
            }

            return props;
        }

        /// <summary>
        ///     Sets the field name.
        /// </summary>
        /// <param name="name">The value to set the field name to.</param>
        /// <returns>
        ///     The current builder.
        /// </returns>
        public SlashCommandBuilder WithName(string name)
        {
            Name = name;
            return this;
        }

        /// <summary>
        ///     Sets the description of the current command.
        /// </summary>
        /// <param name="description">The description of this command.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandBuilder WithDescription(string description)
        {
            Description = description;
            return this;
        }

        /// <summary>
        ///     Sets the default permission of the current command.
        /// </summary>
        /// <param name="value">The default permission value to set.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandBuilder WithDefaultPermission(bool value)
        {
            DefaultPermission = value;
            return this;
        }

        /// <summary>
        ///     Adds an option to the current slash command.
        /// </summary>
        /// <param name="name">The name of the option to add.</param>
        /// <param name="type">The type of this option.</param>
        /// <param name="description">The description of this option.</param>
        /// <param name="required">If this option is required for this command.</param>
        /// <param name="isDefault">If this option is the default option.</param>
        /// <param name="isAutocomplete">If this option is set to autocomplete.</param>
        /// <param name="options">The options of the option to add.</param>
        /// <param name="channelTypes">The allowed channel types for this option.</param>
        /// <param name="choices">The choices of this option.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandBuilder AddOption(string name, ApplicationCommandOptionType type,
           string description, bool? required = null, bool? isDefault = null, bool isAutocomplete = false, List<SlashCommandOptionBuilder> options = null,
           List<ChannelType> channelTypes = null, params ApplicationCommandOptionChoiceProperties[] choices)
        {
            // Make sure the name matches the requirements from discord
            Preconditions.NotNullOrEmpty(name, nameof(name));
            Preconditions.AtLeast(name.Length, 1, nameof(name));
            Preconditions.AtMost(name.Length, MaxNameLength, nameof(name));

            // Discord updated the docs, this regex prevents special characters like @!$%( and s p a c e s.. etc,
            // https://discord.com/developers/docs/interactions/slash-commands#applicationcommand
            if (!Regex.IsMatch(name, @"^[\w-]{1,32}$"))
                throw new ArgumentException("Command name cannot contain any special characters or whitespaces!", nameof(name));

            // same with description
            Preconditions.NotNullOrEmpty(description, nameof(description));
            Preconditions.AtLeast(description.Length, 1, nameof(description));
            Preconditions.AtMost(description.Length, MaxDescriptionLength, nameof(description));

            // make sure theres only one option with default set to true
            if (isDefault == true && Options?.Any(x => x.Default == true) == true)
                throw new ArgumentException("There can only be one command option with default set to true!", nameof(isDefault));

            var option = new SlashCommandOptionBuilder
            {
                Name = name,
                Description = description,
                Required = required,
                Default = isDefault,
                Options = options,
                Type = type,
                Autocomplete = isAutocomplete,
                Choices = (choices ?? Array.Empty<ApplicationCommandOptionChoiceProperties>()).ToList(),
                ChannelTypes = channelTypes
            };

            return AddOption(option);
        }

        /// <summary>
        ///     Adds an option to the current slash command.
        /// </summary>
        /// <param name="name">The name of the option to add.</param>
        /// <param name="type">The type of this option.</param>
        /// <param name="description">The description of this option.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandBuilder AddOption(string name, ApplicationCommandOptionType type, string description)
            => AddOption(name, type, description, options: null, choices: null);

        /// <summary>
        ///     Adds an option to this slash command.
        /// </summary>
        /// <param name="option">The option to add.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandBuilder AddOption(SlashCommandOptionBuilder option)
        {
            Options ??= new List<SlashCommandOptionBuilder>();

            if (Options.Count >= MaxOptionsCount)
                throw new InvalidOperationException($"Cannot have more than {MaxOptionsCount} options!");

            Preconditions.NotNull(option, nameof(option));

            Options.Add(option);
            return this;
        }
        /// <summary>
        ///     Adds a collection of options to the current slash command.
        /// </summary>
        /// <param name="options">The collection of options to add.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandBuilder AddOptions(params SlashCommandOptionBuilder[] options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options), "Options cannot be null!");

            if (options.Length == 0)
                throw new ArgumentException("Options cannot be empty!", nameof(options));

            Options ??= new List<SlashCommandOptionBuilder>();

            if (Options.Count + options.Length > MaxOptionsCount)
                throw new ArgumentOutOfRangeException(nameof(options), $"Cannot have more than {MaxOptionsCount} options!");

            Options.AddRange(options);
            return this;
        }
    }

    /// <summary>
    ///     Represents a class used to build options for the <see cref="SlashCommandBuilder"/>.
    /// </summary>
    public class SlashCommandOptionBuilder
    {
        /// <summary>
        ///     The max length of a choice's name allowed by Discord.
        /// </summary>
        public const int ChoiceNameMaxLength = 100;

        /// <summary>
        ///     The maximum number of choices allowed by Discord.
        /// </summary>
        public const int MaxChoiceCount = 25;

        private string _name;
        private string _description;

        /// <summary>
        ///     Gets or sets the name of this option.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (value != null)
                {
                    Preconditions.AtLeast(value.Length, 1, nameof(value));
                    Preconditions.AtMost(value.Length, SlashCommandBuilder.MaxNameLength, nameof(value));
                    if (!Regex.IsMatch(value, @"^[\w-]{1,32}$"))
                        throw new ArgumentException("Option name cannot contain any special characters or whitespaces!", nameof(value));
                }

                _name = value;
            }
        }

        /// <summary>
        ///     Gets or sets the description of this option.
        /// </summary>
        public string Description
        {
            get => _description;
            set
            {
                if (value != null)
                {
                    Preconditions.AtLeast(value.Length, 1, nameof(value));
                    Preconditions.AtMost(value.Length, SlashCommandBuilder.MaxDescriptionLength, nameof(value));
                }

                _description = value;
            }
        }

        /// <summary>
        ///     Gets or sets the type of this option.
        /// </summary>
        public ApplicationCommandOptionType Type { get; set; }

        /// <summary>
        ///     Gets or sets whether or not this options is the first required option for the user to complete. only one option can be default.
        /// </summary>
        public bool? Default { get; set; }

        /// <summary>
        ///     Gets or sets if the option is required.
        /// </summary>
        public bool? Required { get; set; }

        /// <summary>
        ///     Gets or sets whether or not this option supports autocomplete.
        /// </summary>
        public bool Autocomplete { get; set; }

        /// <summary>
        ///     Gets or sets the choices for string and int types for the user to pick from.
        /// </summary>
        public List<ApplicationCommandOptionChoiceProperties> Choices { get; set; }

        /// <summary>
        ///     Gets or sets if this option is a subcommand or subcommand group type, these nested options will be the parameters.
        /// </summary>
        public List<SlashCommandOptionBuilder> Options { get; set; }

        /// <summary>
        ///     Gets or sets the allowed channel types for this option.
        /// </summary>
        public List<ChannelType> ChannelTypes { get; set; }

        /// <summary>
        ///     Builds the current option.
        /// </summary>
        /// <returns>The built version of this option.</returns>
        public ApplicationCommandOptionProperties Build()
        {
            bool isSubType = Type == ApplicationCommandOptionType.SubCommandGroup;

            if (isSubType && (Options == null || !Options.Any()))
                throw new InvalidOperationException("SubCommands/SubCommandGroups must have at least one option");

            if (!isSubType && Options != null && Options.Any() && Type != ApplicationCommandOptionType.SubCommand)
                throw new InvalidOperationException($"Cannot have options on {Type} type");

            return new ApplicationCommandOptionProperties
            {
                Name = Name,
                Description = Description,
                Default = Default,
                Required = Required,
                Type = Type,
                Options = Options?.Count > 0 ? Options.Select(x => x.Build()).ToList() : new List<ApplicationCommandOptionProperties>(),
                Choices = Choices,
                Autocomplete = Autocomplete,
                ChannelTypes = ChannelTypes
            };
        }

        /// <summary>
        ///     Adds an option to the current slash command.
        /// </summary>
        /// <param name="name">The name of the option to add.</param>
        /// <param name="type">The type of this option.</param>
        /// <param name="description">The description of this option.</param>
        /// <param name="required">If this option is required for this command.</param>
        /// <param name="isDefault">If this option is the default option.</param>
        /// <param name="isAutocomplete">If this option supports autocomplete.</param>
        /// <param name="options">The options of the option to add.</param>
        /// <param name="channelTypes">The allowed channel types for this option.</param>
        /// <param name="choices">The choices of this option.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder AddOption(string name, ApplicationCommandOptionType type,
           string description, bool? required = null, bool isDefault = false, bool isAutocomplete = false, List<SlashCommandOptionBuilder> options = null,
           List<ChannelType> channelTypes = null, params ApplicationCommandOptionChoiceProperties[] choices)
        {
            // Make sure the name matches the requirements from discord
            Preconditions.NotNullOrEmpty(name, nameof(name));
            Preconditions.AtLeast(name.Length, 1, nameof(name));
            Preconditions.AtMost(name.Length, SlashCommandBuilder.MaxNameLength, nameof(name));

            // Discord updated the docs, this regex prevents special characters like @!$%( and s p a c e s.. etc,
            // https://discord.com/developers/docs/interactions/slash-commands#applicationcommand
            if (!Regex.IsMatch(name, @"^[\w-]{1,32}$"))
                throw new ArgumentException("Command name cannot contain any special characters or whitespaces!", nameof(name));

            // same with description
            Preconditions.NotNullOrEmpty(description, nameof(description));
            Preconditions.AtLeast(description.Length, 1, nameof(description));
            Preconditions.AtMost(description.Length, SlashCommandBuilder.MaxDescriptionLength, nameof(description));

            // make sure theres only one option with default set to true
            if (isDefault && Options?.Any(x => x.Default == true) == true)
                throw new ArgumentException("There can only be one command option with default set to true!", nameof(isDefault));

            var option = new SlashCommandOptionBuilder
            {
                Name = name,
                Description = description,
                Required = required,
                Default = isDefault,
                Autocomplete = isAutocomplete,
                Options = options,
                Type = type,
                Choices = (choices ?? Array.Empty<ApplicationCommandOptionChoiceProperties>()).ToList(),
                ChannelTypes = channelTypes
            };

            return AddOption(option);
        }
        /// <summary>
        ///     Adds a sub option to the current option.
        /// </summary>
        /// <param name="option">The sub option to add.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder AddOption(SlashCommandOptionBuilder option)
        {
            Options ??= new List<SlashCommandOptionBuilder>();

            if (Options.Count >= SlashCommandBuilder.MaxOptionsCount)
                throw new InvalidOperationException($"There can only be {SlashCommandBuilder.MaxOptionsCount} options per sub command group!");

            Preconditions.NotNull(option, nameof(option));

            Options.Add(option);
            return this;
        }

        /// <summary>
        ///     Adds a choice to the current option.
        /// </summary>
        /// <param name="name">The name of the choice.</param>
        /// <param name="value">The value of the choice.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder AddChoice(string name, int value)
        {
            Choices ??= new List<ApplicationCommandOptionChoiceProperties>();

            if (Choices.Count >= MaxChoiceCount)
                throw new InvalidOperationException($"Cannot add more than {MaxChoiceCount} choices!");

            Preconditions.NotNull(name, nameof(name));

            Preconditions.AtLeast(name.Length, 1, nameof(name));
            Preconditions.AtMost(name.Length, 100, nameof(name));

            Choices.Add(new ApplicationCommandOptionChoiceProperties
            {
                Name = name,
                Value = value
            });

            return this;
        }

        /// <summary>
        ///     Adds a choice to the current option.
        /// </summary>
        /// <param name="name">The name of the choice.</param>
        /// <param name="value">The value of the choice.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder AddChoice(string name, string value)
        {
            Choices ??= new List<ApplicationCommandOptionChoiceProperties>();

            if (Choices.Count >= MaxChoiceCount)
                throw new InvalidOperationException($"Cannot add more than {MaxChoiceCount} choices!");

            Preconditions.NotNull(name, nameof(name));
            Preconditions.NotNull(value, nameof(value));

            Preconditions.AtLeast(name.Length, 1, nameof(name));
            Preconditions.AtMost(name.Length, 100, nameof(name));

            Preconditions.AtLeast(value.Length, 1, nameof(value));
            Preconditions.AtMost(value.Length, 100, nameof(value));

            Choices.Add(new ApplicationCommandOptionChoiceProperties
            {
                Name = name,
                Value = value
            });

            return this;
        }

        /// <summary>
        ///     Adds a channel type to the current option.
        /// </summary>
        /// <param name="channelType">The <see cref="ChannelType"/> to add.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder AddChannelType(ChannelType channelType)
        {
            ChannelTypes ??= new List<ChannelType>();

            ChannelTypes.Add(channelType);

            return this;
        }

        /// <summary>
        ///     Sets the current builders name.
        /// </summary>
        /// <param name="name">The name to set the current option builder.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder WithName(string name)
        {
            Name = name;

            return this;
        }

        /// <summary>
        ///     Sets the current builders description.
        /// </summary>
        /// <param name="description">The description to set.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder WithDescription(string description)
        {
            Description = description;
            return this;
        }

        /// <summary>
        ///     Sets the current builders required field.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder WithRequired(bool value)
        {
            Required = value;
            return this;
        }

        /// <summary>
        ///     Sets the current builders default field.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder WithDefault(bool value)
        {
            Default = value;
            return this;
        }

        /// <summary>
        ///     Sets the current builders autocomplete field.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder WithAutocomplete(bool value)
        {
            Autocomplete = value;
            return this;
        }

        /// <summary>
        ///     Sets the current type of this builder.
        /// </summary>
        /// <param name="type">The type to set.</param>
        /// <returns>The current builder.</returns>
        public SlashCommandOptionBuilder WithType(ApplicationCommandOptionType type)
        {
            Type = type;
            return this;
        }
    }
}
