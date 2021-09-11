using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Represents an executable <see cref="ICommandInfo"/> for a Slash Command
    /// </summary>
    public class SlashCommandParameterInfo : CommandParameterInfo
    {
        /// <inheritdoc/>
        public new SlashCommandInfo Command => base.Command as SlashCommandInfo;

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// 
        /// </summary>
        public TypeReader TypeReader { get; }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationCommandOptionType DiscordOptionType => TypeReader.GetDiscordType();

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyCollection<ParameterChoice> Choices { get; }

        internal SlashCommandParameterInfo (Builders.SlashCommandParameterBuilder builder, SlashCommandInfo command) : base(builder, command)
        {
            TypeReader = builder.TypeReader;
            Description = builder.Description;
            Choices = builder.Choices.ToImmutableArray();
        }
    }
}
