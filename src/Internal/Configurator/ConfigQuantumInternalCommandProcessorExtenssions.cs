using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quantum.Command.Internal.Publisher;
using Quantum.Configurator;
using Quantum.DataBase.EntityFramework;

namespace Quantum.Command.Internal.Configurator;

public static class ConfigQuantumInternalCommandProcessorExtenssions
{
    public static ConfigQuantumInternalCommandProcessorBuilder ConfigQuantumInternalCommandProcessor(this QuantumServiceCollection collection)
    {
        return new ConfigQuantumInternalCommandProcessorBuilder(collection);
    }
}

public class ConfigQuantumInternalCommandProcessorBuilder
{
    private readonly QuantumServiceCollection _collection;

    public ConfigQuantumInternalCommandProcessorBuilder(QuantumServiceCollection collection)
    {
        _collection = collection;
    }

    public QuantumServiceCollection and()
    {
        return _collection;
    }

    public ConfigQuantumInternalCommandProcessorBuilder RegisterInternalPipelineAsScoped<T>(T pipeline)
        where T : class, IQuantumInternalCommandPipeline
    {
        _collection.Collection.AddScoped<IQuantumInternalCommandPipeline>(sp => pipeline);
        return this;
    }

    public ConfigQuantumInternalCommandProcessorBuilder RegisterInternalPipelineAsScoped(Func<IServiceProvider, IQuantumInternalCommandPipeline> factory)
    {
        _collection.Collection.AddScoped(factory);
        return this;
    }

    public ConfigQuantumInternalCommandProcessorBuilder RegisterPipeLineStage<T>(Func<IServiceProvider, object> factory, ServiceLifetime scoped)
    {
        _collection.Collection.Add(new ServiceDescriptor(typeof(T), factory, scoped));
        return this;
    }

    public ConfigQuantumInternalCommandProcessorBuilder RegisterPipeLineStage<T>(ServiceLifetime serviceLifetime)
        where T : class
    {
        _collection.Collection.Add(new ServiceDescriptor(typeof(T), typeof(T), serviceLifetime));
        return this;
    }

    public ConfigQuantumInternalCommandProcessorBuilder RegisterCommandsSchedulerDbContextAsTransient()
    {
        _collection.Collection.AddTransient<CommandsSchedulerDbContext>();
        return this;
    }

    public ConfigQuantumInternalCommandProcessorBuilder RegisterCommandsSchedulerDbContext<T>(ServiceLifetime serviceLifeTime)
        where T : DbContext
    {
        this._collection.Collection.Add(new ServiceDescriptor(typeof(CommandsSchedulerDbContext), typeof(T), serviceLifeTime));
        return this;
    }

    public ConfigQuantumInternalCommandProcessorBuilder WithOptions(DbContextOptions<QuantumDbContext> options)
    {
        _collection.Collection.AddSingleton(options);
        return this;
    }

    public ConfigQuantumInternalCommandProcessorBuilder RegisterDomainEventPublisherAsScoped()
    {
        _collection.Collection.AddScoped<IDomainEventPublisher, DomainEventPublisher>();

        return this;
    }

    public ConfigQuantumInternalCommandProcessorBuilder RegisterDomainEventPublisher<T>(ServiceLifetime serviceLifetime)
        where T : class, IDomainEventPublisher
    {
        this._collection.Collection.Add(new ServiceDescriptor(typeof(IDomainEventPublisher), typeof(T), serviceLifetime));

        return this;
    }

    public ConfigQuantumInternalCommandProcessorBuilder RegisterInternalServiceExecutor<T>()
        where T : class, IInternalServiceExecutor
    {

        _collection.Collection.AddScoped<IInternalServiceExecutor, T>();

        return this;
    }

    public ConfigQuantumInternalCommandProcessorBuilder RegisterCommandsScheduler<T>()
        where T : class, ICommandsScheduler
    {

        _collection.Collection.AddScoped<ICommandsScheduler, T>();

        return this;
    }

    public ConfigQuantumInternalCommandProcessorBuilder RegisterInternalPipeline(Func<IServiceProvider, IQuantumInternalCommandPipeline> func, ServiceLifetime serviceLifetime)
    {
        this._collection.Collection.Add(new ServiceDescriptor(typeof(IQuantumInternalCommandPipeline), func,serviceLifetime));

        return this;
    }
}