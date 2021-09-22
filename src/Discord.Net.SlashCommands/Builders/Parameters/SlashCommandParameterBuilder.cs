using System;
using System.Collections.Generic;

namespace Discord.SlashCommands.Builders
{
    internal class SlashCommandParameterBuilder : ParameterBuilder<SlashCommandParameterInfo, SlashCommandParameterBuilder>
    {
        private readonly List<ParameterChoice> _choices;

        public string Description { get; set; }
        public IReadOnlyCollection<ParameterChoice> Choices => _choices;
        public TypeConverter TypeConverter { get; private set; }
        protected override SlashCommandParameterBuilder Instance => this;

        internal SlashCommandParameterBuilder ( ICommandBuilder command ) : base(command)
        {
            _choices = new List<ParameterChoice>();
        }

        public SlashCommandParameterBuilder WithDescription (string description)
        {
            Description = description;
            return this;
        }

        public SlashCommandParameterBuilder WithChoices (params ParameterChoice[] options)
        {
            _choices.AddRange(options);
            return this;
        }

        public override SlashCommandParameterBuilder SetParameterType (Type type)
        {
            base.SetParameterType(type);
            TypeConverter = Command.Module.CommandService.GetTypeConverter(ParameterType);
            return this;
        }

        public override SlashCommandParameterInfo Build (ICommandInfo command) =>
            new SlashCommandParameterInfo(this, command as SlashCommandInfo);
    }
}
