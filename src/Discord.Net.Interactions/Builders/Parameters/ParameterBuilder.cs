using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders
{
    public abstract class ParameterBuilder<TInfo, TBuilder> : IParameterBuilder
        where TInfo : class, IParameterInfo
        where TBuilder : ParameterBuilder<TInfo, TBuilder>
    {
        private readonly List<ParameterPreconditionAttribute> _preconditions;
        private readonly List<Attribute> _attributes;

        public ICommandBuilder Command { get; }
        public string Name { get; internal set; }
        public Type ParameterType { get; private set; }
        public bool IsRequired { get; set; } = true;
        public bool IsParameterArray { get; set; } = false;
        public object DefaultValue { get; set; }
        public IReadOnlyCollection<Attribute> Attributes => _attributes;
        public IReadOnlyCollection<ParameterPreconditionAttribute> Preconditions => _preconditions;
        protected abstract TBuilder Instance { get; }

        internal ParameterBuilder ( ICommandBuilder command)
        {
            _attributes = new List<Attribute>();
            _preconditions = new List<ParameterPreconditionAttribute>();

            Command = command;
        }

        public ParameterBuilder (ICommandBuilder command, string name, Type type) : this(command)
        {
            Name = name;
            SetParameterType(type);
        }

        public virtual TBuilder WithName (string name)
        {
            Name = name;
            return Instance;
        }

        public virtual TBuilder SetParameterType (Type type)
        {
            ParameterType = type;
            return Instance;
        }

        public virtual TBuilder SetRequired (bool isRequired)
        {
            IsRequired = isRequired;
            return Instance;
        }

        public virtual TBuilder SetDefaultValue (object defaultValue)
        {
            DefaultValue = defaultValue;
            return Instance;
        }

        public virtual TBuilder AddAttributes (params Attribute[] attributes)
        {
            _attributes.AddRange(attributes);
            return Instance;
        }

        public virtual TBuilder AddPreconditions (params ParameterPreconditionAttribute[] attributes)
        {
            _preconditions.AddRange(attributes);
            return Instance;
        }

        internal abstract TInfo Build (ICommandInfo command);

        IParameterBuilder IParameterBuilder.WithName (string name) =>
            WithName(name);
        IParameterBuilder IParameterBuilder.SetParameterType (Type type) =>
            SetParameterType(type);
        IParameterBuilder IParameterBuilder.SetRequired (bool isRequired) =>
            SetRequired(isRequired);
        IParameterBuilder IParameterBuilder.SetDefaultValue (object defaultValue) =>
            SetDefaultValue(defaultValue);
        IParameterBuilder IParameterBuilder.AddAttributes (params Attribute[] attributes) =>
            AddAttributes(attributes);
        IParameterBuilder IParameterBuilder.AddPreconditions (params ParameterPreconditionAttribute[] preconditions) =>
            AddPreconditions(preconditions);
    }
}
