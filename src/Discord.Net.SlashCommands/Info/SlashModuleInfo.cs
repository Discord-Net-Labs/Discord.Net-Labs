using Discord.SlashCommands.Builders;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Contains the information of a Slash command module
    /// </summary>
    public class SlashModuleInfo
    {
        /// <summary>
        /// Command service this module belongs to
        /// </summary>
        public SlashCommandService CommandService { get; }
        /// <summary>
        /// Get the name of this module that will be displayed on Discord
        /// </summary>
        /// <remarks>
        /// This value may be missing if the commands are registered as standalone
        /// </remarks>
        public string Name { get; }
        /// <summary>
        /// Description of this module
        /// </summary>
        /// <remarks>
        /// This value may be missing if the commands are registered as standalone
        /// </remarks>
        public string Description { get; }
        /// <summary>
        /// Check if the Application Command for this module can be executed by default
        /// </summary>
        public bool DefaultPermission { get; }
        [Obsolete]
        public IReadOnlyList<SlashModuleInfo> SubModules { get; }
        /// <summary>
        /// Get the information list of the Commands that belong to this module
        /// </summary>
        public IReadOnlyList<SlashCommandInfo> Commands { get; }
        /// <summary>
        /// Get the information list of the Interactions that belong to this module
        /// </summary>
        public IReadOnlyCollection<SlashInteractionInfo> Interactions { get; }
        [Obsolete]
        public SlashModuleInfo Parent { get; }
        /// <summary>
        /// Get a list of the attributes of this module
        /// </summary>
        public IReadOnlyList<Attribute> Attributes { get; }

        internal SlashModuleInfo (SlashModuleBuilder builder, SlashCommandService commandService = null)
        {
            CommandService = commandService ?? builder.CommandService;

            Name = builder.Name;
            Description = builder.Description;
            DefaultPermission = builder.DefaultPermission;
            Commands = BuildCommands(builder).ToImmutableArray();
            Interactions = BuildInteractions(builder).ToImmutableArray();
            Attributes = BuildAttributes(builder).ToImmutableArray();
        }

        private IEnumerable<SlashCommandInfo> BuildCommands (SlashModuleBuilder builder)
        {
            var result = new List<SlashCommandInfo>();

            foreach (SlashCommandBuilder commandBuilder in builder.Commands)
                result.Add(commandBuilder.Build(this, CommandService));

            return result;
        }

        private IEnumerable<SlashInteractionInfo> BuildInteractions(SlashModuleBuilder builder)
        {
            var result = new List<SlashInteractionInfo>();

            foreach (var interactionBuilder in builder.Interactions)
                result.Add(interactionBuilder.Build(this, CommandService));

            return result;
        }

        private IEnumerable<Attribute> BuildAttributes (SlashModuleBuilder builder)
        {
            var result = new List<Attribute>();
            var currentParent = builder;

            while (currentParent != null)
            {
                result.AddRange(currentParent.Attributes);
                currentParent = currentParent.Parent;
            }

            return result;
        }
    }
}
