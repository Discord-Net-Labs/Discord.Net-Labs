using Discord.Interactions.Builders;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.Interactions
{
    /// <summary>
    ///     Contains the information of a Interactions Module
    /// </summary>
    public class ModuleInfo
    {
        internal ILookup<string, PreconditionAttribute> GroupedPreconditions { get; }

        /// <summary>
        ///     The underlying command service
        /// </summary>
        public InteractionService CommandService { get; }

        /// <summary>
        ///     Name of this module class
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Group name of this module, if the module is marked with a <see cref="GroupAttribute"/>
        /// </summary>
        public string SlashGroupName { get; }

        /// <summary>
        ///     <see langword="true"/> if this module is marked with a <see cref="GroupAttribute"/>
        /// </summary>
        public bool IsSlashGroup => !string.IsNullOrEmpty(SlashGroupName);

        /// <summary>
        ///     Description of this module if <see cref="IsSlashGroup"/> is <see langword="true"/>
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     Default Permission of this module
        /// </summary>
        public bool DefaultPermission { get; }

        /// <summary>
        ///     Get the collection of Sub Modules of this module
        /// </summary>
        public IReadOnlyList<ModuleInfo> SubModules { get; }

        /// <summary>
        ///     Get the Slash Commands that are declared in this module
        /// </summary>
        public IReadOnlyList<SlashCommandInfo> SlashCommands { get; }

        /// <summary>
        ///     Get the Context Commands that are declared in this module
        /// </summary>
        public IReadOnlyList<ContextCommandInfo> ContextCommands { get; }
        /// <summary>
        ///     Get the Component Commands that are declared in this module
        /// </summary>
        public IReadOnlyCollection<ComponentCommandInfo> ComponentCommands { get; }

        /// <summary>
        ///     Get the declaring type of this module, if <see cref="IsSubModule"/> is <see langword="true"/>
        /// </summary>
        public ModuleInfo Parent { get; }

        /// <summary>
        ///     <see langword="true"/> if this module is declared by another <see cref="InteractionModuleBase{T}"/>
        /// </summary>
        public bool IsSubModule => Parent != null;

        /// <summary>
        ///     Get a collection of the attributes of this module
        /// </summary>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        ///     Get a collection of the preconditions of this module
        /// </summary>
        public IReadOnlyCollection<PreconditionAttribute> Preconditions { get; }

        /// <summary>
        ///     <see langword="true"/> if this module has a valid <see cref="GroupAttribute"/> and has no parent with a <see cref="GroupAttribute"/>
        /// </summary>
        public bool IsTopLevelGroup { get; }

        /// <summary>
        ///     If <see langword="true"/>, this module will not be registered by <see cref="InteractionService.RegisterCommandsGloballyAsync(bool)"/>
        ///     or <see cref="InteractionService.RegisterCommandsToGuildAsync(ulong, bool)"/> methods
        /// </summary>
        public bool DontAutoRegister { get; }

        internal ModuleInfo (ModuleBuilder builder, InteractionService commandService = null, ModuleInfo parent = null)
        {
            CommandService = commandService ?? builder.CommandService;

            Name = builder.Name;
            SlashGroupName = builder.SlashGroupName;
            Description = builder.Description;
            Parent = parent;
            DefaultPermission = builder.DefaultPermission;
            SlashCommands = BuildSlashCommands(builder).ToImmutableArray();
            ContextCommands = BuildContextCommands(builder).ToImmutableArray();
            ComponentCommands = BuildComponentCommands(builder).ToImmutableArray();
            SubModules = BuildSubModules(builder).ToImmutableArray();
            ;
            Attributes = BuildAttributes(builder).ToImmutableArray();
            Preconditions = BuildPreconditions(builder).ToImmutableArray();
            IsTopLevelGroup = CheckTopLevel(parent);
            DontAutoRegister = builder.DontAutoRegister;

            GroupedPreconditions = builder.Preconditions.ToLookup(x => x.Group, x => x, StringComparer.Ordinal);
        }

        private IEnumerable<ModuleInfo> BuildSubModules (ModuleBuilder builder, InteractionService commandService = null)
        {
            var result = new List<ModuleInfo>();

            foreach (Builders.ModuleBuilder moduleBuilder in builder.SubModules)
                result.Add(moduleBuilder.Build(commandService, this));

            return result;
        }

        private IEnumerable<SlashCommandInfo> BuildSlashCommands (ModuleBuilder builder)
        {
            var result = new List<SlashCommandInfo>();

            foreach (Builders.SlashCommandBuilder commandBuilder in builder.SlashCommands)
                result.Add(commandBuilder.Build(this, CommandService));

            return result;
        }

        private IEnumerable<ContextCommandInfo> BuildContextCommands (ModuleBuilder builder)
        {
            var result = new List<ContextCommandInfo>();

            foreach (Builders.ContextCommandBuilder commandBuilder in builder.ContextCommands)
                result.Add(commandBuilder.Build(this, CommandService));

            return result;
        }

        private IEnumerable<ComponentCommandInfo> BuildComponentCommands (ModuleBuilder builder)
        {
            var result = new List<ComponentCommandInfo>();

            foreach (var interactionBuilder in builder.ComponentCommands)
                result.Add(interactionBuilder.Build(this, CommandService));

            return result;
        }

        private IEnumerable<Attribute> BuildAttributes (ModuleBuilder builder)
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

        private IEnumerable<PreconditionAttribute> BuildPreconditions (ModuleBuilder builder)
        {
            var preconditions = new List<PreconditionAttribute>();

            var parent = builder.Parent;

            while (parent != null)
            {
                preconditions.AddRange(parent.Preconditions);
                parent = parent.Parent;
            }

            return preconditions;
        }

        private bool CheckTopLevel (ModuleInfo parent)
        {
            var currentParent = parent;

            while (currentParent != null)
            {
                if (currentParent.IsTopLevelGroup)
                    return false;

                currentParent = currentParent.Parent;
            }
            return true;
        }
    }
}
