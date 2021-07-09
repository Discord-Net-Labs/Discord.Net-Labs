using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Discord.SlashCommands.Builders
{
    public class SlashModuleBuilder
    {
        private readonly List<Attribute> _attributes;
        private readonly List<SlashModuleBuilder> _subModules;
        private readonly List<SlashCommandBuilder> _commands;
        private readonly List<SlashInteractionBuilder> _interactions;

        public SlashCommandService CommandService { get; }
        public SlashModuleBuilder Parent { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool DefaultPermission { get; set; } = true;

        public IReadOnlyList<Attribute> Attributes => _attributes;
        public IReadOnlyList<SlashModuleBuilder> SubModules => _subModules;
        public IReadOnlyList<SlashCommandBuilder> Commands => _commands;
        public IReadOnlyList<SlashInteractionBuilder> Interactions => _interactions;

        internal TypeInfo TypeInfo { get; set; }

        internal SlashModuleBuilder(SlashCommandService commandService, SlashModuleBuilder parent = null)
        {
            CommandService = commandService;
            Parent = parent;

            _attributes = new List<Attribute>();
            _subModules = new List<SlashModuleBuilder>();
            _commands = new List<SlashCommandBuilder>();
            _interactions = new List<SlashInteractionBuilder>();
        }

        public SlashModuleBuilder WithName (string name)
        {
            Name = name;
            return this;
        }

        public SlashModuleBuilder WithDescription(string description)
        {
            Description = description;
            return this;
        }

        public SlashModuleBuilder WithDefaultPermision(bool permission)
        {
            DefaultPermission = permission;
            return this;
        }

        public SlashModuleBuilder AddAttributes(params Attribute[] attributes)
        {
            _attributes.AddRange(attributes);
            return this;
        }

        public SlashModuleBuilder AddCommand (Action<SlashCommandBuilder> configure, Func<ISlashCommandContext, object[], IServiceProvider,
            SlashCommandInfo, Task> callback )
        {
            var command = new SlashCommandBuilder(this, callback);
            configure(command);
            _commands.Add(command);
            return this;
        }

        public SlashModuleBuilder AddCommand (Action<SlashCommandBuilder> configure)
        {
            var command = new SlashCommandBuilder(this);
            configure(command);
            _commands.Add(command);
            return this;
        }

        public SlashModuleBuilder AddInteraction(Action<SlashInteractionBuilder> configure)
        {
            var command = new SlashInteractionBuilder(this);
            configure(command);
            _interactions.Add(command);
            return this;
        }

        public SlashModuleBuilder AddModule ( string name, string description, Action<SlashModuleBuilder> configure)
        {
            var subModule = new SlashModuleBuilder(CommandService, this);
            configure(subModule);
            _subModules.Add(subModule);
            return this;
        }

        internal SlashModuleInfo Build ( SlashCommandService commandService = null )
        {
            return new SlashModuleInfo(this, commandService);
        }
    }
}
