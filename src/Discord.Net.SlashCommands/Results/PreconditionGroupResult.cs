using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.SlashCommands
{
    public class PreconditionGroupResult : PreconditionResult
    {
        public IReadOnlyCollection<PreconditionResult> Results { get; }

        private PreconditionGroupResult (SlashCommandError? error, string reason, IEnumerable<PreconditionResult> results) : base(error, reason)
        {
            Results = results?.ToImmutableArray();
        }

        public static new PreconditionGroupResult FromSuccess ( ) =>
            new PreconditionGroupResult(null, null, null);

        public static new PreconditionGroupResult FromError (Exception exception) =>
            new PreconditionGroupResult(SlashCommandError.Exception, exception.Message, null);

        public static new PreconditionGroupResult FromError (IResult result) =>
            new PreconditionGroupResult(result.Error, result.ErrorReason, null);

        public static new PreconditionGroupResult FromError (string reason, IEnumerable<PreconditionResult> results) =>
            new PreconditionGroupResult(SlashCommandError.UnmetPrecondition, reason, results);
    }
}
