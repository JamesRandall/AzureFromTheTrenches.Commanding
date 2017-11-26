using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal class CommandActorChainExecuter : ICommandActorChainExecuter
    {
        private readonly ConcurrentDictionary<Type, Delegate> _commandActorExecuters =
            new ConcurrentDictionary<Type, Delegate>();

        public async Task<CommandChainHandlerResult<TResult>> ExecuteAsync<TResult>(ICommandChainHandler handler, ICommand<TResult> command, TResult previousResult)
        {
            // we compile this expression to enable command actors to be written with a strongly typed
            // command type syntax e.g.:
            //  class MyCommandActor : ICommandChainHandler<MyCommand, MyResult>
            // Without this command actors would need to be of the form:
            //  class MyCommandActor : ICommandChainHandler<ICommand<MyResult>>
            // Which would lead to lots of casting inside actors. During registration of commands we can guarantee
            // type safety.

            Delegate dlg = _commandActorExecuters.GetOrAdd(handler.GetType(), (actorType) =>
            {
                Type castCommandActor = typeof(ICommandHandler<,>);
                Type[] typeArgs = new[] { command.GetType(), typeof(TResult) };
                Type genericType = castCommandActor.MakeGenericType(typeArgs);

                MethodInfo methodInfo = genericType.GetRuntimeMethod("ExecuteAsync", typeArgs);
                ParameterExpression actorParameter = Expression.Parameter(typeof(ICommandChainHandler));
                ParameterExpression commandParameter = Expression.Parameter(typeof(ICommand<TResult>));
                ParameterExpression previousResultParameter = Expression.Parameter(typeof(TResult));

                var lambda = Expression.Lambda<Func<ICommandHandler, ICommand<TResult>, TResult, Task<CommandChainHandlerResult<TResult>>>>(
                    Expression.Call(Expression.Convert(actorParameter, genericType),
                        methodInfo,
                        Expression.Convert(commandParameter, command.GetType()), previousResultParameter),
                    actorParameter, commandParameter, previousResultParameter);
                Func<ICommandHandler, ICommand<TResult>, TResult, Task<CommandChainHandlerResult<TResult>>> executer = lambda.Compile();
                return executer;
            });

            Func<ICommandChainHandler, ICommand<TResult>, TResult, Task<CommandChainHandlerResult<TResult>>> func = (Func<ICommandChainHandler, ICommand<TResult>, TResult, Task<CommandChainHandlerResult<TResult>>>)dlg;
            return await func(handler, command, previousResult);
        }
    }
}
