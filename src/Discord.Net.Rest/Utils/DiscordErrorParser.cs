using Discord.API;
using Discord.Rest.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Rest
{
    internal class DiscordErrorParser
    {
        public static void Parse(string outgoingJson, DiscordError error)
        {
            if (error.Code == 0)
                throw new Exception();
            if (!int.TryParse((error.Code + "").Substring(0, 2), out var errorPrefix))
                throw new Exception("Somthing has gone terribly wrong");
            throw errorPrefix switch
            {
                10 => new UnknownEntitieException(), 
                20 => new GeneralActionException(),
                30 => new LimitReachedException(),
                40 => new GeneralRequestException(outgoingJson, error),
                50 => new PrecondionException(),
                60 => new MFAException(),
                80 => new UserSearchException(),
                90 => new ReactionException(),
                13 => new APIException(),
                15 => new StageException(),
                16 => new ReplyAndThreadException(),
                17 => new StickerException(),
                _ => new Exception("not implemented"),
            };
        }
    }
}
