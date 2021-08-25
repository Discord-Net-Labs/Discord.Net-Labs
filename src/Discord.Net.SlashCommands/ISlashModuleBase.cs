using Discord.SlashCommands.Builders;

namespace Discord.SlashCommands
{
    internal interface ISlashModuleBase
    {
        void SetContext (ISlashCommandContext context);

        void BeforeExecute (ExecutableInfo command);

        void AfterExecute (ExecutableInfo command);

        void OnModuleBuilding (SlashCommandService commandService, ModuleInfo module);
    }
}
