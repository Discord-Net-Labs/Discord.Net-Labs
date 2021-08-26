using Discord.API;
using Discord.API.Rest;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.SlashCommands
{
    internal static class SlashCommandRestUtil
    {
        // Parameters
        public static ApplicationCommandOptionProperties ParseApplicationCommandOptionProps(this SlashParameterInfo parameterInfo)
        {
            var props = new ApplicationCommandOptionProperties
            {
                Name = parameterInfo.Name,
                Description = parameterInfo.Description,
                Type = parameterInfo.DiscordOptionType,
                Required = parameterInfo.IsRequired,
                Choices = parameterInfo.Choices?.Select(x => new ApplicationCommandOptionChoiceProperties
                {
                    Name = x.Name,
                    Value = x.Value
                })?.ToList()
            };
            parameterInfo.TypeReader.Write(props);

            return props;
        }

        // Commmands

        public static ApplicationCommandProperties ParseApplicationCommandProps (this SlashCommandInfo commandInfo) =>
            new SlashCommandProperties
            {
                Name = commandInfo.Name,
                Description = commandInfo.Description,
                DefaultPermission = commandInfo.DefaultPermission,
                Options = commandInfo.Parameters.Select(x => x.ParseApplicationCommandOptionProps())?.ToList() ?? Optional<List<ApplicationCommandOptionProperties>>.Unspecified
            };

        public static ApplicationCommandOptionProperties ParseApplicationCommandOptionProps (this SlashCommandInfo commandInfo) =>
            new ApplicationCommandOptionProperties
            {
                Name = commandInfo.Name,
                Description = commandInfo.Description,
                Type = ApplicationCommandOptionType.SubCommand,
                Required = false,
                Options = commandInfo.Parameters?.Select(x => x.ParseApplicationCommandOptionProps())?.ToList()
            };

        public static ApplicationCommandProperties ParseApplicationCommandProps (this ContextCommandInfo commandInfo) =>
            new ContextCommandProperties(commandInfo.CommandType)
            {
                Name = commandInfo.Name
            };

        // Modules

        public static IReadOnlyCollection<ApplicationCommandProperties> ToModel(this ModuleInfo moduleInfo)
        {
            var args = new List<ApplicationCommandProperties>();

            ParseModuleModel(args, moduleInfo);
            return args;
        }

        private static void ParseModuleModel(List<ApplicationCommandProperties> args, ModuleInfo moduleInfo)
        {
            args.AddRange(moduleInfo.ContextCommands?.Select(x => x.ParseApplicationCommandProps()));

            if (!moduleInfo.IsSlashGroup)
            {
                args.AddRange(moduleInfo.SlashCommands?.Select(x => x.ParseApplicationCommandProps()));

                foreach (var submodule in moduleInfo.SubModules)
                    ParseModuleModel(args, submodule);
            }
            else
            {
                var options = new List<ApplicationCommandOptionProperties>();

                options.AddRange(moduleInfo.SlashCommands?.Select(x => x.ParseApplicationCommandOptionProps()));
                options.AddRange(moduleInfo.SubModules?.SelectMany(x => x.ParseSubModule(args)));

                args.Add(new SlashCommandProperties
                {
                    Name = moduleInfo.SlashGroupName.ToLower(),
                    Description = moduleInfo.Description,
                    DefaultPermission = moduleInfo.DefaultPermission,
                    Options = options
                });
            }
        }

        private static IReadOnlyCollection<ApplicationCommandOptionProperties> ParseSubModule(this ModuleInfo moduleInfo, List<ApplicationCommandProperties> args)
        {
            args.AddRange(moduleInfo.ContextCommands?.Select(x => x.ParseApplicationCommandProps()));

            var options = new List<ApplicationCommandOptionProperties>();
            options.AddRange(moduleInfo.SlashCommands?.Select(x => x.ParseApplicationCommandOptionProps()));
            options.AddRange(moduleInfo.SubModules?.SelectMany(x => x.ParseSubModule(args)));

            if (!moduleInfo.IsSubModule)
                return options;
            else
                return new List<ApplicationCommandOptionProperties>() { new ApplicationCommandOptionProperties
                {
                    Name = moduleInfo.SlashGroupName.ToLower(),
                    Description = moduleInfo.Description,
                    Type = ApplicationCommandOptionType.SubCommandGroup,
                    Options = options
                } };
        }

        public static ApplicationCommandProperties ToCreationProps(this IApplicationCommand command)
        {
            switch (command.Type)
            {
                case ApplicationCommandType.Slash:
                    return new SlashCommandProperties
                    {
                        Name = command.Name,
                        Description = command.Description,
                        DefaultPermission = command.DefaultPermission,
                        Options = command.Options?.Select(x => x.ToCreationProps())?.ToList() ?? Optional<List<ApplicationCommandOptionProperties>>.Unspecified
                    };
                case ApplicationCommandType.User:
                case ApplicationCommandType.Message:
                    return new ContextCommandProperties(command.Type)
                    {
                        Name = command.Name
                    };
                default:
                    throw new InvalidOperationException($"Cannot create command properties for command type {command.Type}");
            }
        }

        public static ApplicationCommandOptionProperties ToCreationProps(this IApplicationCommandOption commandOption)
            => new ApplicationCommandOptionProperties
            {
                Name = commandOption.Name,
                Description = commandOption.Description,
                Type = commandOption.Type,
                Required = commandOption.Required,
                Choices = commandOption.Choices?.Select(x => new ApplicationCommandOptionChoiceProperties
                {
                    Name = x.Name,
                    Value = x.Value
                }).ToList(),
                Options = commandOption.Options?.Select(x => x.ToCreationProps()).ToList()
            };
    }

    internal sealed class ContextCommandProperties : ApplicationCommandProperties
    {
        internal override ApplicationCommandType Type { get; }

        public ContextCommandProperties ( ApplicationCommandType type )
        {
            Type = type;
        }
    }
}
