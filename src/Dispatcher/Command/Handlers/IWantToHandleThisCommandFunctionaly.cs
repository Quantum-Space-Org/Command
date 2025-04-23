using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Quantum.DataBase.EntityFramework;
using Quantum.Dispatcher.Command;

namespace Quantum.Command.Command.Handlers;

public abstract class IWantToHandleThisCommandFunctionaly<TCommand>(QuantumDbContext dbContext)
    : IWantToHandleThisCommand<TCommand>
    where TCommand : IAmACommand
{
    private Tuple<Type, object> entity;

    private EntityState _state = EntityState.Unchanged;

    protected ApplicationServiceRulesChecker<IWantToHandleThisCommandFunctionaly<TCommand>> If => new(this);

    public IWantToHandleThisCommandFunctionaly<TCommand> Create<T>(Func<T> func)
    {
        var entityValue = func.Invoke();

        entity = new Tuple<Type, object>(typeof(T), entityValue);

        _state = EntityState.Added;
        return this;
    }

    public async Task<IWantToHandleThisCommandFunctionaly<TCommand>> Reconstitute<T>(
        Expression<Func<T, bool>> expression) where T : class
    {
        var entityValue = await dbContext.Get<T>().FirstOrDefaultAsync(expression);

        entity = new Tuple<Type, object>(typeof(T), entityValue);

        return this;
    }

    public async Task<IWantToHandleThisCommandFunctionaly<TCommand>> Reconstitute<T>(object id)
        where T : class
    {
        var entityValue = await dbContext.Get<T>().FindAsync(id);

        entity = new Tuple<Type, object>(typeof(T), entityValue);

        return this;
    }

    public IWantToHandleThisCommandFunctionaly<TCommand> Mutate<T>(Action<T> action)
    {
        action.Invoke((T)entity.Item2);
        _state = EntityState.Modified;

        return this;
    }

    public void Thanks()
    {
        if (entity is not null)
            dbContext.Entry(entity.Item2).State = _state;
    }
}