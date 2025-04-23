namespace Quantum.Dispatcher.Configurator;

using Microsoft.Extensions.DependencyInjection;
using Quantum.Configurator;
using Pipeline;
using static Pipeline.HeyPipeline;

public static class ConfigQuantumPipelineExtensions
{
    public static ConfigQuantumPipelineBuilder ConfigQuantumPipeline(this QuantumServiceCollection collection) 
        => new ConfigQuantumPipelineBuilder(collection);
}

public class ConfigQuantumPipelineBuilder
{
    private readonly QuantumServiceCollection _collection;

    public ConfigQuantumPipelineBuilder(QuantumServiceCollection collection)
    {
        _collection = collection;
    }

    public ConfigQuantumPipelineBuilder RegisterPipeLineStage<T>(ServiceLifetime serviceLifetime)
        where T : class
    {
        _collection.Collection.Add(new ServiceDescriptor(typeof(T), typeof(T), serviceLifetime));
        return this;
    }

    public ConfigQuantumPipelineBuilder RegisterPipelineAsScoped<T>(T pipeline)
        where T : class, IQuantumPipeline
    {
        _collection.Collection.AddScoped<IQuantumPipeline>(sp => pipeline);
        return this;
    }

    public ConfigQuantumPipelineBuilder RegisterPipelineAsScoped<T>(PipelineBuilder builder)
        where T : class, IQuantumPipeline
    {
        _collection.Collection.AddScoped(sp => builder.ThankYou());
        return this;
    }

    public ConfigQuantumPipelineBuilder RegisterPipelineAsScoped(Func<IServiceProvider, IQuantumPipeline> factory)

    {
        _collection.Collection.AddTransient(factory);
        return this;
    }

    public ConfigQuantumPipelineBuilder RegisterPipeline(Func<IServiceProvider, IQuantumPipeline> factory, ServiceLifetime lifetime)
    {
        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                this._collection.Collection.AddSingleton(factory);
                break;
            case ServiceLifetime.Scoped:
                this._collection.Collection.AddScoped(factory);
                break;
            case ServiceLifetime.Transient:
                this._collection.Collection.AddTransient(factory);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
        }

        return this;
    }

    public QuantumServiceCollection and()
    {
        return _collection;
    }

    public ConfigQuantumPipelineBuilder RegisterPipeLineStage<T>(Func<IServiceProvider, object> factory, ServiceLifetime scoped) 
    {
        _collection.Collection.Add(new ServiceDescriptor(typeof(T),factory, scoped));
        return this;
    }
}