using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Discord.SlashCommands.Builders
{
    internal class ModuleBuilder
    {
        private readonly List<Attribute> _attributes;
        private readonly List<ModuleBuilder> _subModules;
        private readonly List<SlashCommandBuilder> _slashCommands;
        private readonly List<ContextCommandBuilder> _contextCommands;
        private readonly List<InteractionBuilder> _interactions;

        public SlashCommandService CommandService { get; }
        public ModuleBuilder Parent { get; }
        public string Name { get; set; }
        public string SlashGroupName { get; set; }
        public bool IsSlashGroup => !string.IsNullOrEmpty(SlashGroupName);
        public string Description { get; set; }
        public bool DefaultPermission { get; set; } = true;

        public IReadOnlyList<Attribute> Attributes => _attributes;
        public IReadOnlyList<ModuleBuilder> SubModules => _subModules;
        public IReadOnlyList<SlashCommandBuilder> SlashCommands => _slashCommands;
        public IReadOnlyList<ContextCommandBuilder> ContextCommands => _contextCommands;
        public IReadOnlyList<InteractionBuilder> Interactions => _interactions;

        internal TypeInfo TypeInfo { get; set; }

        internal ModuleBuilder(SlashCommandService commandService, ModuleBuilder parent = null)
        {
            CommandService = commandService;
            Parent = parent;

            _attributes = new List<Attribute>();
            _subModules = new List<ModuleBuilder>();
            _slashCommands = new List<SlashCommandBuilder>();
            _contextCommands = new List<ContextCommandBuilder>();
            _interactions = new List<InteractionBuilder>();
        }

        public ModuleBuilder WithGroupName (string name)
        {
            SlashGroupName = name;
            return this;
        }

        public ModuleBuilder WithDescription(string description)
        {
            Description = description;
            return this;
        }

        public ModuleBuilder WithDefaultPermision(bool permission)
        {
            DefaultPermission = permission;
            return this;
        }

        public ModuleBuilder AddAttributes(params Attribute[] attributes)
        {
            _attributes.AddRange(attributes);
            return this;
        }

        public ModuleBuilder AddSlashCommand (Action<SlashCommandBuilder> configure)
        {
            var command = new SlashCommandBuilder(this);
            configure(command);
            _slashCommands.Add(command);
            return this;
        }

        public ModuleBuilder AddContextCommand(Action<ContextCommandBuilder> configure)
        {
            var command = new ContextCommandBuilder(this);
            configure(command);
            _contextCommands.Add(command);
            return this;
        }

        public ModuleBuilder AddInteraction(Action<InteractionBuilder> configure)
        {
            var command = new InteractionBuilder(this);
            configure(command);
            _interactions.Add(command);
            return this;
        }

        public ModuleBuilder AddModule ( Action<ModuleBuilder> configure)
        {
            var subModule = new ModuleBuilder(CommandService, this);
            configure(subModule);
            _subModules.Add(subModule);
            return this;
        }

        internal ModuleInfo Build ( SlashCommandService commandService = null, ModuleInfo parent = null )
        {
            return new ModuleInfo(this, commandService, parent);
        }
    }
}
