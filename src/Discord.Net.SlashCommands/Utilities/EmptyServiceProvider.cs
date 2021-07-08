using System;
using System.Collections.Generic;
using System.Text;

namespace Discord.SlashCommands
{
    public class EmptyServiceProvider : IServiceProvider
    {
        public static EmptyServiceProvider Instance => new EmptyServiceProvider();

        public object GetService (Type serviceType) => null;
    }
}
