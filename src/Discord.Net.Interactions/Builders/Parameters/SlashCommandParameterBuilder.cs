using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders
{
    public sealed class SlashCommandParameterBuilder : ParameterBuilder<SlashCommandParameterInfo, SlashCommandParameterBuilder>
    {
        private readonly List<ParameterChoice> _choices = new List<ParameterChoice>();

        public string Description { get; set; }
        public IReadOnlyCollection<ParameterChoice> Choices => _choices;
        public TypeConverter TypeConverter { get; private set; }
        protected override SlashCommandParameterBuilder Instance => this;

        internal SlashCommandParameterBuilder ( ICommandBuilder command ) : base(command) { }

        public SlashCommandParameterBuilder (ICommandBuilder command, string name, Type type) : base(command, name, type) { }

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

        internal override SlashCommandParameterInfo Build (ICommandInfo command) =>
            new SlashCommandParameterInfo(this, command as SlashCommandInfo);
    }
}
