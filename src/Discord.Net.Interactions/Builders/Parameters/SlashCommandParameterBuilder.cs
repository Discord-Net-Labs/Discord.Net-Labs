using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Interactions.Builders
{
    public sealed class SlashCommandParameterBuilder : ParameterBuilder<SlashCommandParameterInfo, SlashCommandParameterBuilder>
    {
        private readonly List<ParameterChoice> _choices = new();
        private readonly List<ChannelType> _channelTypes = new();

        public string Description { get; set; }
        public IReadOnlyCollection<ParameterChoice> Choices => _choices;
        public IReadOnlyCollection<ChannelType> ChannelTypes => _channelTypes;
        public bool Autocomplete { get; set; }
        public TypeConverter TypeConverter { get; private set; }
        public IAutocompleter Autocompleter { get; set; }
        protected override SlashCommandParameterBuilder Instance => this;

        internal SlashCommandParameterBuilder (ICommandBuilder command) : base(command) { }

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

        public SlashCommandParameterBuilder WithChannelTypes (params ChannelType[] channelTypes)
        {
            _channelTypes.AddRange(channelTypes);
            return this;
        }

        public SlashCommandParameterBuilder WithChannelTypes (IEnumerable<ChannelType> channelTypes)
        {
            _channelTypes.AddRange(channelTypes);
            return this;
        }

        public SlashCommandParameterBuilder WithAutocompleter(Type autocompleterType, IServiceProvider services = null)
        {
            Autocompleter = Command.Module.CommandService.GetAutocompleter(autocompleterType, services);
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
