using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Discord.SlashCommands.Builders
{
    internal class InteractionBuilder
    {
        private readonly List<Attribute> _attributes;
        private readonly List<ParameterInfo> _parameters;

        internal Func<ISlashCommandContext, object[], IServiceProvider, ExecutableInfo, Task> Callback { get; set; }

        public ModuleBuilder Module { get; }
        public string Name { get; set; }
        public bool IgnoreGroupNames { get; set; } = false;

        public IReadOnlyList<Attribute> Attributes => _attributes;
        public IReadOnlyList<ParameterInfo> Parameters => _parameters;

        internal InteractionBuilder (ModuleBuilder module, Func<ISlashCommandContext, object[], IServiceProvider, ExecutableInfo, Task> callback) : this(module)
        {
            Callback = callback;
        }

        internal InteractionBuilder (ModuleBuilder module)
        {
            Module = module;

            _attributes = new List<Attribute>();
            _parameters = new List<ParameterInfo>();
        }

        public InteractionBuilder WithName (string name)
        {
            Name = name;
            return this;
        }

        public InteractionBuilder SetIgnoreGroupNames(bool state)
        {
            IgnoreGroupNames = state;
            return this;
        }

        public InteractionBuilder AddAttributes (params Attribute[] attributes)
        {
            _attributes.AddRange(attributes);
            return this;
        }

        public InteractionBuilder AddParameter (ParameterInfo parameter)
        {
            _parameters.Add(parameter);
            return this;
        }

        internal InteractionInfo Build (ModuleInfo module, SlashCommandService commandService) =>
            new InteractionInfo(this, module, commandService);
    }
}
