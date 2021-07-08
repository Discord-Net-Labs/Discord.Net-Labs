using Discord.SlashCommands.Builders;

namespace Discord.SlashCommands
{
    internal interface ISlashModuleBase
    {
        void SetContext (ISlashCommandContext context);

        void BeforeExecute (SlashCommandInfo command);

        void AfterExecute (SlashCommandInfo command);

        void OnModuleBuilding (SlashCommandService commandService, SlashModuleBuilder builder);
    }
}
