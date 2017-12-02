using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal class CommandHandlerChainExecuter : ICommandHandlerChainExecuter
    {
        private readonly ConcurrentDictionary<Type, Delegate> _commandHandlerExecuters =
            new ConcurrentDictionary<Type, Delegate>();

        public async Task<CommandChainHandlerResult<TResult>> ExecuteAsync<TResult>(ICommandChainHandler handler, ICommand<TResult> command, TResult previousResult, CancellationToken cancellationToken)
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
                if (handler is ICommandChainHandler)
                {
                    Func<ICommandChainHandler, ICommand<TResult>, TResult, CancellationToken, Task<CommandChainHandlerResult<TResult>>> executer = CompileCancellableCommandHandlerExecuter(command);
                    return executer;
                }
                else
                {
                    Func<ICommandChainHandler, ICommand<TResult>, TResult, Task<CommandChainHandlerResult<TResult>>> executer = CompileCommandHandlerExecuter(command);
                    return executer;
                }                
            });

            if (handler is ICancellableCommandChainHandler)
            {
                Func<ICommandChainHandler, ICommand<TResult>, TResult, CancellationToken, Task<CommandChainHandlerResult<TResult>>> func = (Func<ICommandChainHandler, ICommand<TResult>, TResult, CancellationToken, Task<CommandChainHandlerResult<TResult>>>)dlg;
                return await func(handler, command, previousResult, cancellationToken);
            }
            else
            {
                Func<ICommandChainHandler, ICommand<TResult>, TResult, Task<CommandChainHandlerResult<TResult>>> func = (Func<ICommandChainHandler, ICommand<TResult>, TResult, Task<CommandChainHandlerResult<TResult>>>)dlg;
                return await func(handler, command, previousResult);
            }            
        }

        private static Func<ICommandChainHandler, ICommand<TResult>, TResult, Task<CommandChainHandlerResult<TResult>>> CompileCommandHandlerExecuter<TResult>(ICommand<TResult> command)
        {
            Type castCommandHandler = typeof(ICommandChainHandler<,>);
            Type[] typeArgs = new[] {command.GetType(), typeof(TResult)};
            Type genericType = castCommandHandler.MakeGenericType(typeArgs);

            MethodInfo methodInfo = genericType.GetRuntimeMethod("ExecuteAsync", typeArgs);
            ParameterExpression handlerParameter = Expression.Parameter(typeof(ICommandChainHandler));
            ParameterExpression commandParameter = Expression.Parameter(typeof(ICommand<TResult>));
            ParameterExpression previousResultParameter = Expression.Parameter(typeof(TResult));

            var lambda =
                Expression.Lambda<Func<ICommandChainHandler, ICommand<TResult>, TResult, Task<CommandChainHandlerResult<TResult>>>>(
                    Expression.Call(Expression.Convert(handlerParameter, genericType),
                        methodInfo,
                        Expression.Convert(commandParameter, command.GetType()), previousResultParameter),
                    handlerParameter, commandParameter, previousResultParameter);
            Func<ICommandChainHandler, ICommand<TResult>, TResult, Task<CommandChainHandlerResult<TResult>>> executer =
                lambda.Compile();
            return executer;
        }

        private static Func<ICommandChainHandler, ICommand<TResult>, TResult, CancellationToken, Task<CommandChainHandlerResult<TResult>>> CompileCancellableCommandHandlerExecuter<TResult>(ICommand<TResult> command)
        {
            Type castCommandHandler = typeof(ICancellableCommandChainHandler<,>);
            Type[] typeArgs = new[] { command.GetType(), typeof(TResult) };
            Type genericType = castCommandHandler.MakeGenericType(typeArgs);

            Type[] methodTypeArgs = new[] { command.GetType(), typeof(TResult), typeof(CancellationToken) };
            MethodInfo methodInfo = genericType.GetRuntimeMethod("ExecuteAsync", methodTypeArgs);
            ParameterExpression handlerParameter = Expression.Parameter(typeof(ICommandChainHandler));
            ParameterExpression commandParameter = Expression.Parameter(typeof(ICommand<TResult>));
            ParameterExpression previousResultParameter = Expression.Parameter(typeof(TResult));
            ParameterExpression cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken));

            var lambda =
                Expression.Lambda<Func<ICommandChainHandler, ICommand<TResult>, TResult, CancellationToken, Task<CommandChainHandlerResult<TResult>>>>(
                    Expression.Call(Expression.Convert(handlerParameter, genericType),
                        methodInfo,
                        Expression.Convert(commandParameter, command.GetType()),
                        previousResultParameter,
                        cancellationTokenParameter),
                    handlerParameter, commandParameter, previousResultParameter, cancellationTokenParameter);
            Func<ICommandChainHandler, ICommand<TResult>, TResult, CancellationToken, Task<CommandChainHandlerResult<TResult>>> executer =
                lambda.Compile();
            return executer;
        }
    }
}
