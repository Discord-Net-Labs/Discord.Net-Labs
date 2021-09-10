using System;
using System.Collections.Generic;

namespace Discord.SlashCommands.Builders
{
    internal abstract class CommandBuilder<TInfo, TBuilder, TParamBuilder> : ICommandBuilder
        where TInfo : class, ICommandInfo
        where TBuilder : CommandBuilder<TInfo, TBuilder, TParamBuilder>
        where TParamBuilder : class, IParameterBuilder
    {
        private readonly List<Attribute> _attributes;
        private readonly List<PreconditionAttribute> _preconditions;
        private readonly List<TParamBuilder> _parameters;

        protected abstract TBuilder Instance { get; }

        public ExecuteCallback Callback { get; set; }
        public ModuleBuilder Module { get; }
        public string Name { get; set; }
        public string MethodName { get; set; }
        public bool IgnoreGroupNames { get; set; }
        public RunMode RunMode { get; set; }
        public IReadOnlyList<Attribute> Attributes => _attributes;
        public IReadOnlyList<TParamBuilder> Parameters => _parameters;
        public IReadOnlyList<PreconditionAttribute> Preconditions => _preconditions;
        IReadOnlyList<IParameterBuilder> ICommandBuilder.Parameters => Parameters;

        public CommandBuilder (ModuleBuilder module)
        {
            _attributes = new List<Attribute>();
            _preconditions = new List<PreconditionAttribute>();
            _parameters = new List<TParamBuilder>();

            Module = module;
        }

        public TBuilder WithName (string name)
        {
            Name = name;
            return Instance;
        }

        public TBuilder WithMethodName (string name)
        {
            MethodName = name;
            return Instance;
        }

        public TBuilder WithAttributes (params Attribute[] attributes)
        {
            _attributes.AddRange(attributes);
            return Instance;
        }

        public TBuilder SetRunMode (RunMode runMode)
        {
            RunMode = runMode;
            return Instance;
        }

        public TBuilder AddParameter (TParamBuilder builder)
        {
            _parameters.Add(builder);
            return Instance;
        }

        public TBuilder AddParameters (params TParamBuilder[] parameters)
        {
            _parameters.AddRange(parameters);
            return Instance;
        }

        public TBuilder WithPreconditions (params PreconditionAttribute[] preconditions)
        {
            _preconditions.AddRange(preconditions);
            return Instance;
        }

        public abstract TBuilder AddParameter (Action<TParamBuilder> configure);

        internal abstract TInfo Build (ModuleInfo module, SlashCommandService commandService);

        ICommandBuilder ICommandBuilder.WithName (string name) =>
            WithName(name);
        ICommandBuilder ICommandBuilder.WithMethodName (string name) =>
            WithMethodName(name);
        ICommandBuilder ICommandBuilder.WithAttributes (params Attribute[] attributes) =>
            WithAttributes(attributes);
        ICommandBuilder ICommandBuilder.SetRunMode (RunMode runMode) =>
            SetRunMode(runMode);
        ICommandBuilder ICommandBuilder.AddParameter (IParameterBuilder builder) =>
            AddParameter(builder as TParamBuilder);
        ICommandBuilder ICommandBuilder.AddParameters (params IParameterBuilder[] parameters) =>
            AddParameters(parameters as TParamBuilder);
        ICommandBuilder ICommandBuilder.WithPreconditions (params PreconditionAttribute[] preconditions) =>
            WithPreconditions(preconditions);
        ICommandInfo ICommandBuilder.Build (ModuleInfo module, SlashCommandService commandService) =>
            Build(module, commandService);
    }

    internal abstract class CommandBuilder
    {

    }
}
