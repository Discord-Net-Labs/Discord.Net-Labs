using System;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public abstract class ParameterPreconditionAttribute : Attribute
    {
        public virtual string ErrorMessage { get; set; } = null;

        public abstract Task<PreconditionResult> CheckRequirementsAsync (ISlashCommandContext context, IParameterInfo parameterInfo, IServiceProvider services);
    }
}
