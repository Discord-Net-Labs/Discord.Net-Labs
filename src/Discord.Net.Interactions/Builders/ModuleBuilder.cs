using System;
using System.Collections.Generic;
using System.Reflection;

namespace Discord.Interactions.Builders
{
    public class ModuleBuilder
    {
        private readonly List<Attribute> _attributes;
        private readonly List<PreconditionAttribute> _preconditions;
        private readonly List<ModuleBuilder> _subModules;
        private readonly List<SlashCommandBuilder> _slashCommands;
        private readonly List<ContextCommandBuilder> _contextCommands;
        private readonly List<ComponentCommandBuilder> _componentCommands;

        public InteractionService CommandService { get; }
        public ModuleBuilder Parent { get; }
        public string Name { get; internal set; }
        public string SlashGroupName { get; set; }
        public bool IsSlashGroup => !string.IsNullOrEmpty(SlashGroupName);
        public string Description { get; set; }
        public bool DefaultPermission { get; set; } = true;
        public bool DontAutoRegister { get; set; } = false;

        public IReadOnlyList<Attribute> Attributes => _attributes;
        public IReadOnlyCollection<PreconditionAttribute> Preconditions => _preconditions;
        public IReadOnlyList<ModuleBuilder> SubModules => _subModules;
        public IReadOnlyList<SlashCommandBuilder> SlashCommands => _slashCommands;
        public IReadOnlyList<ContextCommandBuilder> ContextCommands => _contextCommands;
        public IReadOnlyList<ComponentCommandBuilder> ComponentCommands => _componentCommands;

        internal TypeInfo TypeInfo { get; set; }

        internal ModuleBuilder (InteractionService interactionService, ModuleBuilder parent = null)
        {
            CommandService = interactionService;
            Parent = parent;

            _attributes = new List<Attribute>();
            _subModules = new List<ModuleBuilder>();
            _slashCommands = new List<SlashCommandBuilder>();
            _contextCommands = new List<ContextCommandBuilder>();
            _componentCommands = new List<ComponentCommandBuilder>();
            _preconditions = new List<PreconditionAttribute>();
        }

        public ModuleBuilder (InteractionService interactionService, string name, ModuleBuilder parent = null) : this(interactionService, parent)
        {
            Name = name;
        }

        public ModuleBuilder WithGroupName (string name)
        {
            SlashGroupName = name;
            return this;
        }

        public ModuleBuilder WithDescription (string description)
        {
            Description = description;
            return this;
        }

        public ModuleBuilder WithDefaultPermision (bool permission)
        {
            DefaultPermission = permission;
            return this;
        }

        public ModuleBuilder AddAttributes (params Attribute[] attributes)
        {
            _attributes.AddRange(attributes);
            return this;
        }

        public ModuleBuilder AddPreconditions (params PreconditionAttribute[] preconditions)
        {
            _preconditions.AddRange(preconditions);
            return this;
        }

        public ModuleBuilder AddSlashCommand (Action<SlashCommandBuilder> configure)
        {
            var command = new SlashCommandBuilder(this);
            configure(command);
            _slashCommands.Add(command);
            return this;
        }

        public ModuleBuilder AddContextCommand (Action<ContextCommandBuilder> configure)
        {
            var command = new ContextCommandBuilder(this);
            configure(command);
            _contextCommands.Add(command);
            return this;
        }

        public ModuleBuilder AddComponentCommand (Action<ComponentCommandBuilder> configure)
        {
            var command = new ComponentCommandBuilder(this);
            configure(command);
            _componentCommands.Add(command);
            return this;
        }

        public ModuleBuilder AddModule (Action<ModuleBuilder> configure)
        {
            var subModule = new ModuleBuilder(CommandService, this);
            configure(subModule);
            _subModules.Add(subModule);
            return this;
        }

        internal ModuleInfo Build (InteractionService interactionService, IServiceProvider services, ModuleInfo parent = null)
        {
            var moduleInfo = new ModuleInfo(this, interactionService, parent);

            IInteractionModuleBase instance = ReflectionUtils<IInteractionModuleBase>.CreateObject(TypeInfo, interactionService, services);
            try
            {
                instance.OnModuleBuilding(interactionService, moduleInfo);
            }
            finally
            {
                ( instance as IDisposable )?.Dispose();
            }

            return moduleInfo;
        }
    }
}
