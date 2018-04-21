using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AzureFunctions.Implementation;
using AzureFromTheTrenches.Commanding.AzureFunctions.Model;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Compiler.Implementation
{
    internal class FunctionCompiler
    {
        private readonly IServiceCollection _serviceCollection;

        private readonly Assembly _configurationSourceAssembly;
        private readonly string _outputBinaryFolder;
        private readonly string _outputFunctionFolder;
        private readonly ICommandRegistry _commandRegistry;
        private readonly IAssemblyCompiler _assemblyCompiler;
        private readonly ITriggerReferenceProvider _triggerReferenceProvider;
        private readonly JsonCompiler _jsonCompiler;

        public FunctionCompiler(
            Assembly configurationSourceAssembly,
            string outputBinaryFolder,
            string outputFunctionFolder,
            IAssemblyCompiler assemblyCompiler = null,
            ITriggerReferenceProvider triggerReferenceProvider = null)
        {
            _configurationSourceAssembly = configurationSourceAssembly;
            _outputBinaryFolder = outputBinaryFolder;
            _outputFunctionFolder = outputFunctionFolder;
            _serviceCollection = new ServiceCollection();
            CommandingDependencyResolverAdapter adapter = new CommandingDependencyResolverAdapter(
                (fromType, toInstance) => _serviceCollection.AddSingleton(fromType, toInstance),
                (fromType, toType) => _serviceCollection.AddTransient(fromType, toType),
                (resolveType) => null // we never resolve during compilation
            );
            _commandRegistry = adapter.AddCommanding();
            _assemblyCompiler = assemblyCompiler ?? new AssemblyCompiler();
            _triggerReferenceProvider = triggerReferenceProvider ?? new TriggerReferenceProvider();
            _jsonCompiler = new JsonCompiler();
        }

        public async Task Compile()
        {
            IFunctionAppConfiguration configuration = ConfigurationLocator.FindConfiguration(_configurationSourceAssembly);
            if (configuration == null)
            {
                throw new ConfigurationException($"The assembly {_configurationSourceAssembly.GetName().Name} does not contain a public class implementing the IFunctionAppConfiguration interface");
            }

            string newAssemblyNamespace = $"{_configurationSourceAssembly.GetName().Name}.Functions";
            FunctionHostBuilder builder = new FunctionHostBuilder(_serviceCollection, _commandRegistry);
            configuration.Build(builder);
            PatchInDefaults(builder, newAssemblyNamespace);
            
            IReadOnlyCollection<Assembly> externalAssemblies = GetExternalAssemblies(builder.FunctionDefinitions);
            _assemblyCompiler.Compile(builder.FunctionDefinitions, externalAssemblies, _outputBinaryFolder, $"{newAssemblyNamespace}.dll");
            _jsonCompiler.Compile(builder.FunctionDefinitions, _outputBinaryFolder, newAssemblyNamespace);
        }

        private void PatchInDefaults(FunctionHostBuilder builder, string newAssemblyNamespace)
        {            
            foreach (AbstractFunctionDefinition definition in builder.FunctionDefinitions)
            {
                definition.Namespace = newAssemblyNamespace;
            }
        }

        private IReadOnlyCollection<Assembly> GetExternalAssemblies(
            IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions)
        {
            HashSet<Assembly> assemblies = new HashSet<Assembly>();

            foreach (AbstractFunctionDefinition functionDefinition in functionDefinitions)
            {
                assemblies.Add(_triggerReferenceProvider.GetTriggerReference(functionDefinition));
            }

            foreach (ServiceDescriptor descriptor in _serviceCollection)
            {
                assemblies.Add(descriptor.ServiceType.Assembly);
                if (descriptor.ImplementationType != null)
                {
                    assemblies.Add(descriptor.ImplementationType.Assembly);
                }

                if (descriptor.ImplementationInstance != null)
                {
                    assemblies.Add(descriptor.ImplementationInstance.GetType().Assembly);
                }
            }

            IRegistrationCatalogue catalogue = (IRegistrationCatalogue)_commandRegistry;

            foreach (Type handler in catalogue.GetRegisteredHandlers())
            {
                assemblies.Add(handler.Assembly);
            }

            foreach (Type command in catalogue.GetRegisteredCommands())
            {
                assemblies.Add(command.Assembly);
            }

            // at the moment we can't get the actual dispatcher types without actually calling the function and looking at ther result - needs thought
            return assemblies;
        }
    }
}
