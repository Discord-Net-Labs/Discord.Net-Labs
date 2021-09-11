using System;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class PreconditionAttribute : Attribute
    {
        public string Group { get; set; } = null;

        public virtual string ErrorMessage { get; set; } = null;

        public abstract Task<PreconditionResult> CheckRequirementsAsync (ISlashCommandContext context, ICommandInfo commandInfo, IServiceProvider services);
    }
}
