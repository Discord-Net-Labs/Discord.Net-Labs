using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Discord.SlashCommands.Builders
{
    public class SlashInteractionBuilder
    {
        private readonly List<Attribute> _attributes;
        private readonly List<ParameterInfo> _parameters;

        internal Func<ISlashCommandContext, object[], IServiceProvider, SlashInteractionInfo, Task> Callback { get; set; }

        public SlashModuleBuilder Module { get; }
        public SlashGroupInfo Group { get; private set; }
        public string Name { get; set; }

        public IReadOnlyList<Attribute> Attributes => _attributes;
        public IReadOnlyList<ParameterInfo> Parameters => _parameters;

        internal SlashInteractionBuilder (SlashModuleBuilder module, Func<ISlashCommandContext, object[], IServiceProvider, SlashInteractionInfo, Task> callback,
            SlashGroupInfo group = null) : this(module, group)
        {
            Callback = callback;
        }

        internal SlashInteractionBuilder (SlashModuleBuilder module, SlashGroupInfo group = null)
        {
            Module = module;
            Group = group;

            _attributes = new List<Attribute>();
            _parameters = new List<ParameterInfo>();
        }

        public SlashInteractionBuilder WithName (string name)
        {
            Name = name;
            return this;
        }

        public SlashInteractionBuilder WithGroup (string name, string description)
        {
            Group = new SlashGroupInfo(name, description);
            return this;
        }

        public SlashInteractionBuilder WithGroup (SlashGroupInfo group)
        {
            Group = group;
            return this;
        }

        public SlashInteractionBuilder AddAttributes (params Attribute[] attributes)
        {
            _attributes.AddRange(attributes);
            return this;
        }

        public SlashInteractionBuilder AddParameter (ParameterInfo parameter)
        {
            _parameters.Add(parameter);
            return this;
        }

        internal SlashInteractionInfo Build (SlashModuleInfo module, SlashCommandService commandService) =>
            new SlashInteractionInfo(this, module, commandService);
    }
}
