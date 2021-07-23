namespace Discord.SlashCommands
{
    public struct ParseResult : IResult
    {
        public SlashCommandError? Error { get; }

        public string ErrorReason { get; }

        public bool IsSuccess => !Error.HasValue;
    }
}
