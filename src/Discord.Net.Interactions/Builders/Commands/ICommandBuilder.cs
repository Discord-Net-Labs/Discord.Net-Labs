using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders
{
    public interface ICommandBuilder
    {
        ExecuteCallback Callback { get; }
        ModuleBuilder Module { get; }
        string Name { get; }
        string MethodName { get; set; }
        bool IgnoreGroupNames { get; set; }
        RunMode RunMode { get; set; }
        IReadOnlyList<Attribute> Attributes { get; }
        IReadOnlyList<IParameterBuilder> Parameters { get; }
        IReadOnlyList<PreconditionAttribute> Preconditions { get; }

        ICommandBuilder WithName (string name);
        ICommandBuilder WithMethodName (string name);
        ICommandBuilder WithAttributes (params Attribute[] attributes);
        ICommandBuilder SetRunMode (RunMode runMode);
        ICommandBuilder AddParameters (params IParameterBuilder[] parameters);
        ICommandBuilder WithPreconditions (params PreconditionAttribute[] preconditions);
    }
}
