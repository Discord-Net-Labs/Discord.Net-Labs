using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Discord.SlashCommands
{
    public class CommandParameterInfo : IParameterInfo
    {
        public ICommandInfo Command { get; }

        public string Name { get; }

        public Type ParameterType { get; }

        public bool IsRequired { get; }

        public bool IsParameterArray { get; }

        public object DefaultValue { get; }

        public IReadOnlyCollection<Attribute> Attributes { get; }

        public IReadOnlyCollection<ParameterPreconditionAttribute> Preconditions { get; }

        internal CommandParameterInfo(Builders.IParameterBuilder builder, ICommandInfo command)
        {
            Command = command;
            Name = builder.Name;
            ParameterType = builder.ParameterType;
            IsRequired = builder.IsRequired;
            IsParameterArray = builder.IsParameterArray;
            DefaultValue = builder.DefaultValue;
            Attributes = builder.Attributes.ToImmutableArray();
            Preconditions = builder.Preconditions.ToImmutableArray();
        }

        public async Task<PreconditionResult> CheckPreconditionsAsync (ISlashCommandContext context, IServiceProvider services)
        {
            foreach (var precondition in Preconditions)
            {
                var result = await precondition.CheckRequirementsAsync(context, this, services).ConfigureAwait(false);
                if (!result.IsSuccess)
                    return result;
            }

            return PreconditionResult.FromSuccess();
        }
    }
}
