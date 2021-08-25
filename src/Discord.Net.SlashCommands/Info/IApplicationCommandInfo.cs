namespace Discord.SlashCommands
{
    public interface IApplicationCommandInfo
    {
        string Name { get; }
        ApplicationCommandType CommandType { get; }
    }
}
