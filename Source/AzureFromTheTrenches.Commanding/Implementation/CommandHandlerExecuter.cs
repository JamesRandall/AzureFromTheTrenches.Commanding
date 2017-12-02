using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal class CommandHandlerExecuter : ICommandHandlerExecuter
    {
        private readonly ConcurrentDictionary<Type, Delegate> _commandHandlerExecuters = 
            new ConcurrentDictionary<Type, Delegate>();

        public async Task<TResult> ExecuteAsync<TResult>(ICommandHandler handler, ICommand<TResult> command, TResult previousResult, CancellationToken cancellationToken)
        {
            // we compile this expression to enable command actors to be written with a strongly typed
            // command type syntax e.g.:
            //  class MyCommandActor : ICommandHandler<MyCommand, MyResult>
            // Without this command actors would need to be of the form:
            //  class MyCommandActor : ICommandHandler<ICommand<MyResult>>
            // Which would lead to lots of casting inside actors. During registration of commands we can guarantee
            // type safety.

            if (command is NoResultCommandWrapper wrappedCommand)
            {
                await ExecuteActorForCommandWithNoResult(handler, wrappedCommand, cancellationToken);
                return default(TResult);
            }
            return await ExecuteActorForCommand(handler, command, previousResult, cancellationToken);
        }

        private async Task<TResult> ExecuteActorForCommand<TResult>(ICommandHandler handler, ICommand<TResult> command, TResult previousResult, CancellationToken cancellationToken)
        {
            Delegate dlg = _commandHandlerExecuters.GetOrAdd(handler.GetType(), (handlerType) =>
            {
                if (handler is ICancellableCommandHandler)
                {
                    Func<ICommandHandler, ICommand<TResult>, TResult, CancellationToken, Task<TResult>> executer = CompileCancellableCommandHandlerExecuterWithResult(command);
                    return executer;
                }
                else
                {
                    Func<ICommandHandler, ICommand<TResult>, TResult, Task<TResult>> executer = CompileCommandHandlerExecuterWithResult(command);
                    return executer;
                }                
            });

            if (handler is ICancellableCommandHandler)
            {
                Func<ICommandHandler, ICommand<TResult>, TResult, CancellationToken, Task<TResult>> func = (Func<ICommandHandler, ICommand<TResult>, TResult, CancellationToken, Task<TResult>>)dlg;
                return await func(handler, command, previousResult, cancellationToken);
            }
            else
            {
                Func<ICommandHandler, ICommand<TResult>, TResult, Task<TResult>> func =
                    (Func<ICommandHandler, ICommand<TResult>, TResult, Task<TResult>>)dlg;
                return await func(handler, command, previousResult);
            }
            
        }

        private static Func<ICommandHandler, ICommand<TResult>, TResult, Task<TResult>> CompileCommandHandlerExecuterWithResult<TResult>(ICommand<TResult> command)
        {
            Type castCommandHandler = typeof(ICommandHandler<,>);
            Type[] typeArgs = new[] {command.GetType(), typeof(TResult)};
            Type genericType = castCommandHandler.MakeGenericType(typeArgs);

            MethodInfo methodInfo = genericType.GetRuntimeMethod("ExecuteAsync", typeArgs);
            ParameterExpression handlerParameter = Expression.Parameter(typeof(ICommandHandler));
            ParameterExpression commandParameter = Expression.Parameter(typeof(ICommand<TResult>));
            ParameterExpression previousResultParameter = Expression.Parameter(typeof(TResult));

            var lambda = Expression.Lambda<Func<ICommandHandler, ICommand<TResult>, TResult, Task<TResult>>>(
                Expression.Call(Expression.Convert(handlerParameter, genericType),
                    methodInfo,
                    Expression.Convert(commandParameter, command.GetType()), previousResultParameter),
                handlerParameter, commandParameter, previousResultParameter);
            Func<ICommandHandler, ICommand<TResult>, TResult, Task<TResult>> executer = lambda.Compile();
            return executer;
        }

        private static Func<ICommandHandler, ICommand<TResult>, TResult, CancellationToken, Task<TResult>> CompileCancellableCommandHandlerExecuterWithResult<TResult>(ICommand<TResult> command)
        {
            Type castCommandHandler = typeof(ICancellableCommandHandler<,>);
            Type[] typeArgs = new[] { command.GetType(), typeof(TResult) };
            Type genericType = castCommandHandler.MakeGenericType(typeArgs);

            Type[] methodTypeArgs = new[] { command.GetType(), typeof(TResult), typeof(CancellationToken) };
            MethodInfo methodInfo = genericType.GetRuntimeMethod("ExecuteAsync", methodTypeArgs);
            ParameterExpression handlerParameter = Expression.Parameter(typeof(ICommandHandler));
            ParameterExpression commandParameter = Expression.Parameter(typeof(ICommand<TResult>));
            ParameterExpression previousResultParameter = Expression.Parameter(typeof(TResult));
            ParameterExpression cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken));

            var lambda = Expression.Lambda<Func<ICommandHandler, ICommand<TResult>, TResult, CancellationToken, Task<TResult>>>(
                Expression.Call(Expression.Convert(handlerParameter, genericType),
                    methodInfo,
                    Expression.Convert(commandParameter, command.GetType()),
                    previousResultParameter,
                    cancellationTokenParameter),
                handlerParameter, commandParameter, previousResultParameter, cancellationTokenParameter);
            Func<ICommandHandler, ICommand<TResult>, TResult, CancellationToken, Task<TResult>> executer = lambda.Compile();
            return executer;
        }

        private async Task ExecuteActorForCommandWithNoResult(ICommandHandler handler, NoResultCommandWrapper wrappedCommand, CancellationToken cancellationToken)
        {
            Delegate dlg = _commandHandlerExecuters.GetOrAdd(handler.GetType(), (handlerType) =>
            {
                if (handler is ICancellableCommandHandler)
                {
                    Func<ICommandHandler, ICommand, CancellationToken, Task> executer = CompileCancellableCommandHandlerExecuterWithNoResult(wrappedCommand);
                    return executer;
                }
                else
                {
                    Func<ICommandHandler, ICommand, Task> executer = CompileCommandHandlerExecuterWithNoResult(wrappedCommand);
                    return executer;
                }                
            });
            if (handler is ICancellableCommandHandler)
            {
                Func<ICommandHandler, ICommand, CancellationToken, Task> func = (Func<ICommandHandler, ICommand, CancellationToken, Task>)dlg;
                await func(handler, wrappedCommand.Command, cancellationToken);
            }
            else
            {
                Func<ICommandHandler, ICommand, Task> func = (Func<ICommandHandler, ICommand, Task>)dlg;
                await func(handler, wrappedCommand.Command);
            }
                
        }

        private static Func<ICommandHandler, ICommand, CancellationToken, Task> CompileCancellableCommandHandlerExecuterWithNoResult(NoResultCommandWrapper wrappedCommand)
        {
            Type castCommandHandler = typeof(ICancellableCommandHandler<>);
            Type[] typeArgs = new[] { wrappedCommand.Command.GetType() };
            Type genericType = castCommandHandler.MakeGenericType(typeArgs);

            Type[] methodTypeArgs = new[] { wrappedCommand.Command.GetType(), typeof(CancellationToken) };
            MethodInfo methodInfo = genericType.GetRuntimeMethod("ExecuteAsync", methodTypeArgs);
            ParameterExpression handlerParameter = Expression.Parameter(typeof(ICommandHandler));
            ParameterExpression commandParameter = Expression.Parameter(typeof(ICommand));
            ParameterExpression cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken));

            var lambda = Expression.Lambda<Func<ICommandHandler, ICommand, CancellationToken, Task>>(
                Expression.Call(Expression.Convert(handlerParameter, genericType),
                    methodInfo,
                    Expression.Convert(commandParameter, wrappedCommand.Command.GetType()),
                    cancellationTokenParameter),
                handlerParameter, commandParameter, cancellationTokenParameter);
            Func<ICommandHandler, ICommand, CancellationToken, Task> executer = lambda.Compile();
            return executer;
        }

        private static Func<ICommandHandler, ICommand, Task> CompileCommandHandlerExecuterWithNoResult(NoResultCommandWrapper wrappedCommand)
        {
            
               Type castCommandHandler = typeof(ICommandHandler<>);
            Type[] typeArgs = new[] {wrappedCommand.Command.GetType()};
            Type genericType = castCommandHandler.MakeGenericType(typeArgs);

            MethodInfo methodInfo = genericType.GetRuntimeMethod("ExecuteAsync", typeArgs);
            ParameterExpression handlerParameter = Expression.Parameter(typeof(ICommandHandler));
            ParameterExpression commandParameter = Expression.Parameter(typeof(ICommand));

            var lambda = Expression.Lambda<Func<ICommandHandler, ICommand, Task>>(
                Expression.Call(Expression.Convert(handlerParameter, genericType),
                    methodInfo,
                    Expression.Convert(commandParameter, wrappedCommand.Command.GetType())),
                handlerParameter, commandParameter);
            Func<ICommandHandler, ICommand, Task> executer = lambda.Compile();
            return executer;
        }
    }
}
