using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Discord.SlashCommands.Builders
{
    internal class ContextCommandBuilder
    {
        private readonly List<Attribute> _attributes;
        private readonly List<ParameterInfo> _parameters;

        internal Func<ISlashCommandContext, object[], IServiceProvider, ExecutableInfo, Task> Callback { get; set; }

        public ModuleBuilder Module { get; }
        public string Name { get; set; }
        public ApplicationCommandType CommandType { get; set; }
        public bool DefaultPermission { get; set; } = true;

        public IReadOnlyList<Attribute> Attributes => _attributes;
        public IReadOnlyList<ParameterInfo> Parameters => _parameters;

        internal ContextCommandBuilder (ModuleBuilder module, Func<ISlashCommandContext, object[], IServiceProvider, ExecutableInfo, Task> callback)
            :this(module)
        {
            Callback = callback;
        }

        internal ContextCommandBuilder (ModuleBuilder module)
        {
            Module = module;

            _attributes = new List<Attribute>();
            _parameters = new List<ParameterInfo>();
        }

        public ContextCommandBuilder WithName (string name)
        {
            Name = name;
            return this;
        }

        public ContextCommandBuilder SetType (ApplicationCommandType commandType)
        {
            CommandType = commandType;
            return this;
        }

        public ContextCommandBuilder SetDefaultPermission(bool defaultPermision)
        {
            DefaultPermission = defaultPermision;
            return this;
        }

        public ContextCommandBuilder AddAttributes (params Attribute[] attributes)
        {
            _attributes.AddRange(attributes);
            return this;
        }

        public ContextCommandBuilder AddParameter (ParameterInfo parameter)
        {
            _parameters.Add(parameter);
            return this;
        }

        internal ContextCommandInfo Build (ModuleInfo module, SlashCommandService commandService) =>
            ContextCommandInfo.Create(this, module, commandService);
    }
}
