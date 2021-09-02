using Discord.SlashCommands.Builders;
using Discord.WebSocket;
using System;
using System.Collections.Generic;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Contains the information of a parameter of a Slash Command
    /// </summary>
    public class SlashParameterInfo
    {
        /// <summary>
        /// Command this paramter belongs to
        /// </summary>
        public SlashCommandInfo Command { get; }

        /// <summary>
        /// Get the name of this parameter that will be shown on Discord
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Get the description of this parameter that will be shown on Discord
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Type of this parameter
        /// </summary>
        public Type ParameterType { get; }

        /// <summary>
        /// If <see langword="true"/> this parameter will no be optional
        /// </summary>
        public bool IsRequired { get; }

        /// <summary>
        /// Default value of this parameter if the parameter is optional
        /// </summary>
        public object DefaultValue { get; }

        /// <summary>
        /// Predefined set of values for the user to choose from
        /// </summary>
        public IReadOnlyList<ParameterChoice> Choices { get; }

        /// <summary>
        /// Get a list of the attributes this parameter has
        /// </summary>
        public IReadOnlyList<Attribute> Attributes { get; }

        /// <summary>
        /// Parameter type that will be registered to Discord if applicable
        /// </summary>
        public ApplicationCommandOptionType DiscordOptionType => TypeReader.GetDiscordType();

        /// <summary>
        /// TypeReader that will be used to handle this parameter
        /// </summary>
        public TypeReader TypeReader { get; }

        internal SlashParameterInfo (SlashParameterBuilder builder, SlashCommandInfo command)
        {
            Command = command;
            Name = builder.Name;
            Description = builder.Description;
            ParameterType = builder.ParameterType;
            IsRequired = builder.IsRequired;
            DefaultValue = builder.DefaultValue;
            Choices = builder.Choices;
            Attributes = builder.Attributes;
            TypeReader = builder.TypeReader;
        }

        public override string ToString ( ) => Name;
    }
}
