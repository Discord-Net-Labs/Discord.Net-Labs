using System;
using System.Collections.Generic;
using System.Text;

namespace Discord.SlashCommands
{
    public abstract class RuntimeResult : IResult
    {
        public SlashCommandError? Error { get; }

        public string ErrorReason { get; }

        public bool IsSuccess => !Error.HasValue;

        protected RuntimeResult (SlashCommandError? error, string reason )
        {
            Error = error;
            ErrorReason = reason;
        }

        public override string ToString ( ) => ErrorReason ?? ( IsSuccess ? "Successful" : "Unsuccessful" );
    }
}
