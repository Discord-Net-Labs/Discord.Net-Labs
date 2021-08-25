using Discord.API;
using Discord.API.Rest;
using System.Collections.Generic;
using System.Linq;

namespace Discord.SlashCommands
{
    internal static class SlashCommandRestUtil
    {
        // Parameters
        public static API.ApplicationCommandOption ParseApplicationCommandOption (this SlashParameterInfo parameterInfo)
        {

            var props = new ApplicationCommandOptionProperties();
            parameterInfo.TypeReader.Write(props);

            var option = new API.ApplicationCommandOption
            {
                Name = props.Name ?? parameterInfo.Name.ToLower(),
                Description = props.Description ?? parameterInfo.Description.ToLower(),
                Required = props.Required ?? parameterInfo.IsRequired,
                Type = parameterInfo.DiscordOptionType,
                Choices = props.Choices != null ? props.Choices.Select(x => new ApplicationCommandOptionChoice
                {
                    Name = x.Name,
                    Value = x.Value
                }).ToArray() :
                parameterInfo.Choices?.Select(x => new ApplicationCommandOptionChoice
                {
                    Name = x.Name,
                    Value = x.Value
                }).ToArray(),
                Options = null
            };

            if (option.Choices.IsSpecified && option.Choices.Value.Count() == 0)
                option.Choices = null;

            return option;
        }

        // Commmands

        public static CreateApplicationCommandParams ParseApplicationCommandParams(this SlashCommandInfo commandInfo)
        {
            return new CreateApplicationCommandParams(commandInfo.Name.ToLower(), commandInfo.Description, commandInfo.CommandType)
            {
                DefaultPermission = commandInfo.DefaultPermission,
                Options = commandInfo.Parameters?.Select(x => x.ParseApplicationCommandOption()).ToArray()
            };
        }

        public static CreateApplicationCommandParams ParseApplicationCommandParas(this ContextCommandInfo commandInfo)
        {
            return new CreateApplicationCommandParams(commandInfo.Name, null, commandInfo.CommandType)
            {
                DefaultPermission = commandInfo.DefaultPermission
            };
        }

        public static API.ApplicationCommandOption ParseApplicationCommandOption (this SlashCommandInfo commandInfo)
        {
            var option = new API.ApplicationCommandOption
            {
                Name = commandInfo.Name.ToLower(),
                Description = commandInfo.Description,
                Type = ApplicationCommandOptionType.SubCommand,
                Options = commandInfo.Parameters.Select(x => x.ParseApplicationCommandOption()).ToArray(),
                Choices = null,
                Required = false
            };

            if (!option.Options.IsSpecified)
                option.Options = null;

            return option;
        }

        // Modules

        public static IReadOnlyCollection<CreateApplicationCommandParams> ToModel(this ModuleInfo moduleInfo)
        {
            var args = new List<CreateApplicationCommandParams>();

            ParseModuleModel(args, moduleInfo);
            return args;
        }

        private static void ParseModuleModel(List<CreateApplicationCommandParams> args, ModuleInfo moduleInfo)
        {
            args.AddRange(moduleInfo.ContextCommands.Select(x => x.ParseApplicationCommandParas()));

            if (string.IsNullOrEmpty(moduleInfo.SlashGroupName))
            {
                args.AddRange(moduleInfo.SlashCommands.Select(x => x.ParseApplicationCommandParams()));

                foreach (var submodule in moduleInfo.SubModules)
                    ParseModuleModel(args, submodule);
            }
            else
            {
                var options = new List<ApplicationCommandOption>();

                options.AddRange(moduleInfo.SlashCommands.Select(x => x.ParseApplicationCommandOption()));
                options.AddRange(moduleInfo.SubModules.SelectMany(x => x.ParseSubModule(args)));

                args.Add(new CreateApplicationCommandParams
                {
                    Name = moduleInfo.SlashGroupName.ToLower(),
                    Description = moduleInfo.Description,
                    DefaultPermission = moduleInfo.DefaultPermission,
                    Type = ApplicationCommandType.Slash,
                    Options = options.ToArray()
                });
            }
        }

        private static IReadOnlyCollection<ApplicationCommandOption> ParseSubModule(this ModuleInfo module, List<CreateApplicationCommandParams> args)
        {
            args.AddRange(module.ContextCommands.Select(x => x.ParseApplicationCommandParas()));

            var options = new List<ApplicationCommandOption>();

            options.AddRange(module.SlashCommands.Select(x => x.ParseApplicationCommandOption()));
            options.AddRange(module.SubModules.SelectMany(x => x.ParseSubModule(args)));

            if (string.IsNullOrEmpty(module.SlashGroupName))
                return options;
            else
                return new List<ApplicationCommandOption>() {
                    new ApplicationCommandOption
                    {
                        Name = module.SlashGroupName.ToLower(),
                        Description = module.Description,
                        Type = ApplicationCommandOptionType.SubCommandGroup,
                        Options = options.ToArray()
                    }
                };
        }
    }
}
