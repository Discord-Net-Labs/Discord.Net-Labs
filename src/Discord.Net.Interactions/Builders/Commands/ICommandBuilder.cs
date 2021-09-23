using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders
{
    internal interface ICommandBuilder
    {
        ExecuteCallback Callback { get; set; }
        ModuleBuilder Module { get; }
        string Name { get; set; }
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
        ICommandBuilder AddParameter (IParameterBuilder builder);
        ICommandBuilder AddParameters (params IParameterBuilder[] parameters);
        ICommandBuilder WithPreconditions (params PreconditionAttribute[] preconditions);
        ICommandInfo Build (ModuleInfo module, InteractionService commandService);
    }
}
