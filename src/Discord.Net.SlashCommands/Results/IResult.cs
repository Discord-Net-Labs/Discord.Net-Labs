namespace Discord.SlashCommands
{
    public interface IResult
    {
        SlashCommandError? Error { get; }
        string ErrorReason { get; }
        bool IsSuccess { get; }
    }
}
