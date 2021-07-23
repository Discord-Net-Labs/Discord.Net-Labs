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
            var option = new API.ApplicationCommandOption
            {
                Name = parameterInfo.Name.ToLower(),
                Description = parameterInfo.Description.ToLower(),
                Required = parameterInfo.IsRequired,
                Type = parameterInfo.DiscordOptionType,
                Choices = parameterInfo.Choices != null ?
                parameterInfo.Choices.Select(x => new ApplicationCommandOptionChoice
                {
                    Name = x.Name,
                    Value = x.Value
                }).ToArray() : null,
                Options = null
            };

            if (option.Choices.IsSpecified && option.Choices.Value.Count() == 0)
                option.Choices = null;

            return option;
        }

        // Commmands

        public static CreateApplicationCommandParams ParseApplicationCommandParams(this SlashCommandInfo commandInfo)
        {
            return new CreateApplicationCommandParams(commandInfo.Name, commandInfo.Description)
            {
                DefaultPermission = commandInfo.DefaultPermission,
                Options = commandInfo.Parameters?.Select(x => x.ParseApplicationCommandOption()).ToArray()
            };
        }

        public static API.ApplicationCommandOption ParseApplicationCommandOption (this SlashCommandInfo commandInfo)
        {
            var option = new API.ApplicationCommandOption
            {
                Name = commandInfo.Name.ToLower(),
                Description = commandInfo.Description.ToLower(),
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

        public static IReadOnlyCollection<CreateApplicationCommandParams> ToModel(this SlashModuleInfo moduleInfo)
        {
            var args = new List<CreateApplicationCommandParams>();

            ParseModuleModel(args, moduleInfo);
            return args;
        }

        private static void ParseModuleModel(List<CreateApplicationCommandParams> args, SlashModuleInfo moduleInfo)
        {
            if (string.IsNullOrEmpty(moduleInfo.SlashGroupName))
            {
                args.AddRange(moduleInfo.Commands.Select(x => x.ParseApplicationCommandParams()));

                foreach (var submodule in moduleInfo.SubModules)
                    ParseModuleModel(args, submodule);
            }
            else
            {
                var options = new List<ApplicationCommandOption>();

                options.AddRange(moduleInfo.Commands.Select(x => x.ParseApplicationCommandOption()));
                options.AddRange(moduleInfo.SubModules.SelectMany(x => x.ParseSubModule()));

                args.Add(new CreateApplicationCommandParams
                {
                    Name = moduleInfo.SlashGroupName,
                    Description = moduleInfo.Description,
                    DefaultPermission = moduleInfo.DefaultPermission,
                    Options = options.ToArray()
                });
            }
        }

        private static IReadOnlyCollection<ApplicationCommandOption> ParseSubModule(this SlashModuleInfo module)
        {
            var options = new List<ApplicationCommandOption>();

            options.AddRange(module.Commands.Select(x => x.ParseApplicationCommandOption()));
            options.AddRange(module.SubModules.SelectMany(x => x.ParseSubModule()));

            if (string.IsNullOrEmpty(module.SlashGroupName))
                return options;
            else
                return new List<ApplicationCommandOption>() {
                    new ApplicationCommandOption
                    {
                        Name = module.SlashGroupName,
                        Description = module.Description,
                        Type = ApplicationCommandOptionType.SubCommandGroup,
                        Options = options.ToArray()
                    }
                };
        }
    }
}
