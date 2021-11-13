using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the parameter info class for <see cref="SlashCommandInfo"/> commands
    /// </summary>
    public class SlashCommandParameterInfo : CommandParameterInfo
    {
        /// <inheritdoc/>
        public new SlashCommandInfo Command => base.Command as SlashCommandInfo;

        /// <summary>
        ///     Description of the Slash Command Parameter
        /// </summary>
        public string Description { get; }

        public double? MinValue { get; }
        public double? MaxValue { get; }

        /// <summary>
        ///     <see cref="TypeConverter{T}"/> that will be used to convert the incoming <see cref="Discord.WebSocket.SocketSlashCommandDataOption"/> into
        ///     <see cref="CommandParameterInfo.ParameterType"/>
        /// </summary>
        public TypeConverter TypeConverter { get; }

        public IAutocompleter Autocompleter { get; }

        public bool Autocomplete => Autocompleter is not null;

        /// <summary>
        ///     Discord option type this parameter represents
        /// </summary>
        public ApplicationCommandOptionType DiscordOptionType => TypeConverter.GetDiscordType();

        /// <summary>
        ///     Get the parameter choices of this Slash Application Command parameter
        /// </summary>
        public IReadOnlyCollection<ParameterChoice> Choices { get; }

        /// <summary>
        ///     Gets the allowed channel types for this option.
        /// </summary>
        public IReadOnlyCollection<ChannelType> ChannelTypes { get; }

        internal SlashCommandParameterInfo(Builders.SlashCommandParameterBuilder builder, SlashCommandInfo command) : base(builder, command)
        {
            TypeConverter = builder.TypeConverter;
            Autocompleter = builder.Autocompleter;
            Description = builder.Description;
            MaxValue = builder.MaxValue;
            MinValue = builder.MinValue;
            Choices = builder.Choices.ToImmutableArray();
            ChannelTypes = builder.ChannelTypes.ToImmutableArray();
        }
    }
}
