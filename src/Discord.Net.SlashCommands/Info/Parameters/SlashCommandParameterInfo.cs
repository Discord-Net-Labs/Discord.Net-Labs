using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.SlashCommands
{
    public class SlashCommandParameterInfo : CommandParameterInfo
    {
        public new SlashCommandInfo Command => base.Command as SlashCommandInfo;
        public string Description { get; }
        public TypeReader TypeReader { get; }
        public ApplicationCommandOptionType DiscordOptionType => TypeReader.GetDiscordType();
        public IReadOnlyCollection<ParameterChoice> Choices { get; }

        internal SlashCommandParameterInfo (Builders.SlashCommandParameterBuilder builder, SlashCommandInfo command) : base(builder, command)
        {
            TypeReader = builder.TypeReader;
            Description = builder.Description;
            Choices = builder.Choices.ToImmutableArray();
        }
    }
}
