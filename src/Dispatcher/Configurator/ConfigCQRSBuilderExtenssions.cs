using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Quantum.Command.Command.Handlers;
using Quantum.Configurator;
using Quantum.Dispatcher.Semaphore;

namespace Quantum.Dispatcher.Configurator;

public static class ConfigCQRSBuilderExtenssions
{
    public static ConfigCQRSBuilder ConfigCQRS(this QuantumServiceCollection collection)
    {
        return new ConfigCQRSBuilder(collection);
    }
}

public class ConfigCQRSBuilder
{
    private readonly QuantumServiceCollection _collection;

    public ConfigCQRSBuilder(QuantumServiceCollection collection)
    {
        _collection = collection;
    }

    public ConfigCQRSBuilder RegisterCommandHandlersInAssemblyAsTransient(Assembly assembly)
    {
        var commandHandlers =
            assembly.GetTypes()
                .Where(t =>
                    t.BaseType != null &&
                    t.BaseType.Name == typeof(IWantToHandleThisCommand<>).Name);

        foreach (var commandHandler in commandHandlers)
        {
            _collection.Collection.AddTransient(commandHandler.BaseType, commandHandler);
        }


        return this;
    }

    public ConfigCQRSBuilder RegisterCommandHandlersInAssembliesAsTransient(params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            RegisterCommandHandlersInAssemblyAsTransient(assembly);
        }


        return this;
    }

    public ConfigCQRSBuilder RegisterSemaphoreAsScoped<T>()
        where T : class, ISemaphore
    {
        _collection.Collection.AddScoped<ISemaphore, T>();
        return this;
    }

    public QuantumServiceCollection and() => _collection;
}