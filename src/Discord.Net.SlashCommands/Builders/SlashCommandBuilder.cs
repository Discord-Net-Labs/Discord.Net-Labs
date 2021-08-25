using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.SlashCommands.Builders
{
    internal class SlashCommandBuilder
    {
        private List<Attribute> _attributes;
        private List<SlashParameterBuilder> _parameters;

        internal Func<ISlashCommandContext, object[], IServiceProvider, ExecutableInfo, Task> Callback { get; set; }

        public ModuleBuilder Module { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool DefaultPermission { get; set; } = true;
        public bool IgnoreGroupNames { get; set; } = false;

        public IReadOnlyList<Attribute> Attributes => _attributes;
        public IReadOnlyList<SlashParameterBuilder> Parameters => _parameters;

        internal SlashCommandBuilder (ModuleBuilder module, Func<ISlashCommandContext, object[], IServiceProvider, ExecutableInfo, Task> callback)
            : this(module)
        {
            Callback = callback;
        }

        internal SlashCommandBuilder (ModuleBuilder module)
        {
            Module = module;

            _attributes = new List<Attribute>();
            _parameters = new List<SlashParameterBuilder>();
        }

        public SlashCommandBuilder WithName (string name)
        {
            Name = name;
            return this;
        }

        public SlashCommandBuilder WithDescription (string description)
        {
            Description = description;
            return this;
        }

        public SlashCommandBuilder WithDefaultPermission(bool permission)
        {
            DefaultPermission = permission;
            return this;
        }

        public SlashCommandBuilder SetIgnoreGroupNames(bool state)
        {
            IgnoreGroupNames = state;
            return this;
        }

        public SlashCommandBuilder AddAttributes (params Attribute[] attributes)
        {
            _attributes.AddRange(attributes);
            return this;
        }

        public SlashCommandBuilder AddParameter<T> (string name, Action<SlashParameterBuilder> configure) =>
            AddParameter(name, typeof(T), configure);

        public SlashCommandBuilder AddParameter (string name, Type type, Action<SlashParameterBuilder> configure)
        {
            var parameter = new SlashParameterBuilder(this, name, type);
            configure(parameter);
            _parameters.Add(parameter);
            return this;
        }

        public SlashCommandBuilder AddParameter(Action<SlashParameterBuilder> configure)
        {
            var parameter = new SlashParameterBuilder(this);
            configure(parameter);
            _parameters.Add(parameter);
            return this;
        }

        internal SlashCommandInfo Build (ModuleInfo module, SlashCommandService commandService) =>
            new SlashCommandInfo(this, module, commandService);
    }
}
