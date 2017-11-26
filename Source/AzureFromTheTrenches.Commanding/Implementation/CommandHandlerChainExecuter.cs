using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal class CommandHandlerChainExecuter : ICommandHandlerChainExecuter
    {
        private readonly ConcurrentDictionary<Type, Delegate> _commandHandlerExecuters =
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

            Delegate dlg = _commandHandlerExecuters.GetOrAdd(handler.GetType(), (handlerType) =>
            {
                Type castCommandHandler = typeof(ICommandHandler<,>);
                Type[] typeArgs = new[] { command.GetType(), typeof(TResult) };
                Type genericType = castCommandHandler.MakeGenericType(typeArgs);

                MethodInfo methodInfo = genericType.GetRuntimeMethod("ExecuteAsync", typeArgs);
                ParameterExpression handlerParameter = Expression.Parameter(typeof(ICommandChainHandler));
                ParameterExpression commandParameter = Expression.Parameter(typeof(ICommand<TResult>));
                ParameterExpression previousResultParameter = Expression.Parameter(typeof(TResult));

                var lambda = Expression.Lambda<Func<ICommandHandler, ICommand<TResult>, TResult, Task<CommandChainHandlerResult<TResult>>>>(
                    Expression.Call(Expression.Convert(handlerParameter, genericType),
                        methodInfo,
                        Expression.Convert(commandParameter, command.GetType()), previousResultParameter),
                    handlerParameter, commandParameter, previousResultParameter);
                Func<ICommandHandler, ICommand<TResult>, TResult, Task<CommandChainHandlerResult<TResult>>> executer = lambda.Compile();
                return executer;
            });

            Func<ICommandChainHandler, ICommand<TResult>, TResult, Task<CommandChainHandlerResult<TResult>>> func = (Func<ICommandChainHandler, ICommand<TResult>, TResult, Task<CommandChainHandlerResult<TResult>>>)dlg;
            return await func(handler, command, previousResult);
        }
    }
}
