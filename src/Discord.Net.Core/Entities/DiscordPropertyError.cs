using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public class DiscordPropertyError
    {
        public string Name { get; }
        public object Value { get; }
        public string Path { get; }

        internal DiscordPropertyError()
        {

        }
    }
}
