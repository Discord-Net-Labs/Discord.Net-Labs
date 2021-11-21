using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic parsed json error received from discord after preforming a rest request.
    /// </summary>
    public struct DiscordJsonError
    {
        public string Path { get; }
        public IReadOnlyCollection<DiscordError> Errors { get; }

        internal DiscordJsonError(string path, DiscordError[] errors)
        {
            Path = path;
            Errors = errors.ToImmutableArray();
        }
    }

    public struct DiscordError
    {
        public string Code { get; }
        public string Message { get; }

        internal DiscordError(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
