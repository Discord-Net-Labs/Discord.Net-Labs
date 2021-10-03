using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.API.Rest;
using Discord.API;

namespace Discord.Rest.Exceptions    
{
    public class GeneralRequestException : Exception
    {
        internal GeneralRequestException(string json, DiscordError value)
        {
            
        }
    }
}
