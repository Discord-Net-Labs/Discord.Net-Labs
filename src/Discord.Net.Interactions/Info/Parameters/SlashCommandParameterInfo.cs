using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Interactions
{
    /// <summary>
    /// Represents an executable <see cref="ICommandInfo"/> for a Slash Command
    /// </summary>
    public class SlashCommandParameterInfo : CommandParameterInfo
    {
        /// <inheritdoc/>
        public new SlashCommandInfo Command => base.Command as SlashCommandInfo;

        /// <summary>
        /// Description of the Slash Command Parameter
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// <see cref="TypeConverter{T}"/> that will be used to convert the incoming <see cref="Discord.WebSocket.SocketSlashCommandDataOption"/> into <see cref="CommandParameterInfo.ParameterType"/>
        /// </summary>
        public TypeConverter TypeConverter { get; }

        /// <summary>
        /// Discord option type this parameter represents
        /// </summary>
        public ApplicationCommandOptionType DiscordOptionType => TypeConverter.GetDiscordType();

        /// <summary>
        /// Get the collection of the choices this parameter has
        /// </summary>
        public IReadOnlyCollection<ParameterChoice> Choices { get; }

        internal SlashCommandParameterInfo (Builders.SlashCommandParameterBuilder builder, SlashCommandInfo command) : base(builder, command)
        {
            TypeConverter = builder.TypeConverter;
            Description = builder.Description;
            Choices = builder.Choices.ToImmutableArray();
        }
    }
}
