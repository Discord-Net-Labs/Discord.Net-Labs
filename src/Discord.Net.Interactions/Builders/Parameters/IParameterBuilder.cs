using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders
{
    public interface IParameterBuilder
    {
        ICommandBuilder Command { get; }
        string Name { get; }
        Type ParameterType { get; }
        bool IsRequired { get; }
        bool IsParameterArray { get; }
        object DefaultValue { get; }
        IReadOnlyCollection<Attribute> Attributes { get; }
        IReadOnlyCollection<ParameterPreconditionAttribute> Preconditions { get; }

        IParameterBuilder WithName (string name);
        IParameterBuilder SetParameterType (Type type);
        IParameterBuilder SetRequired (bool isRequired);
        IParameterBuilder SetDefaultValue (object defaultValue);
        IParameterBuilder AddAttributes (params Attribute[] attributes);
        IParameterBuilder AddPreconditions (params ParameterPreconditionAttribute[] preconditions);
    }
}
