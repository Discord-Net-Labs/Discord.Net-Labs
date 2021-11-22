using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public struct CurrentUser
    {
        public bool? Verified { get; set; }
        public string Email { get; set; }
        public bool? MfaEnabled { get; set; }
        public UserProperties? Flags { get; set; }
        public PremiumType? PremiumType { get; set; }
        public string Locale { get; set; }
        public UserProperties? PublicFlags { get; set; }
    }
}
