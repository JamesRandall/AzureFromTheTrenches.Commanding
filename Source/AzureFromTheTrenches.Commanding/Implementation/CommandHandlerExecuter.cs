using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal class CommandHandlerExecuter : ICommandHandlerExecuter
    {
        private readonly Dictionary<Type, Delegate> _commandHandlerExecuters = 
            new Dictionary<Type, Delegate>();

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

        public void CompileHandlerExecuter(Type commandType, Type commandHandlerType)
        {
            Type cancellableCommandHandlerType = typeof(ICancellableCommandHandler);
            Type commandWithResultGenericType = typeof(ICommand<>);
            Type commandWithResultType = commandType.GetInterfaces().SingleOrDefault(x =>
                x.IsGenericType && x.GetGenericTypeDefinition() == commandWithResultGenericType);
            if (commandWithResultType != null)
            {
                Type resultType = commandWithResultType.GetGenericArguments().Single();
                Type commandInterfaceType = commandWithResultGenericType.MakeGenericType(resultType); // ICommand<TResult>

                if (cancellableCommandHandlerType.IsAssignableFrom(commandHandlerType))
                {
                    CompileCancellableCommandWithResult(commandType, commandHandlerType, resultType, commandInterfaceType);
                }
                else
                {
                    CompileCommandWithResult(commandType, commandHandlerType, resultType, commandInterfaceType);
                }
            }
            else
            {
                if (cancellableCommandHandlerType.IsAssignableFrom(commandHandlerType))
                {
                    CompileCancellableCommandWithNoResult(commandType, commandHandlerType);
                }
                else
                {
                    CompileCommandWithNoResult(commandType, commandHandlerType);
                }
            }
        }

        private async Task<TResult> ExecuteActorForCommand<TResult>(ICommandHandler handler, ICommand<TResult> command, TResult previousResult, CancellationToken cancellationToken)
        {
            Delegate dlg = _commandHandlerExecuters[handler.GetType()];
            
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

        private async Task ExecuteActorForCommandWithNoResult(ICommandHandler handler, NoResultCommandWrapper wrappedCommand, CancellationToken cancellationToken)
        {
            Delegate dlg = _commandHandlerExecuters[handler.GetType()];

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

        private void CompileCommandWithNoResult(Type commandType, Type commandHandlerType)
        {
            Type castCommandHandler = typeof(ICommandHandler<>);
            Type[] typeArgs = new[] { commandType };
            Type genericType = castCommandHandler.MakeGenericType(typeArgs);

            MethodInfo methodInfo = genericType.GetRuntimeMethod("ExecuteAsync", typeArgs);
            ParameterExpression handlerParameter = Expression.Parameter(typeof(ICommandHandler));
            ParameterExpression commandParameter = Expression.Parameter(typeof(ICommand));

            var lambda = Expression.Lambda<Func<ICommandHandler, ICommand, Task>>(
                Expression.Call(Expression.Convert(handlerParameter, genericType),
                    methodInfo,
                    Expression.Convert(commandParameter, commandType)),
                handlerParameter, commandParameter);
            Func<ICommandHandler, ICommand, Task> executer = lambda.Compile();
            _commandHandlerExecuters.Add(commandHandlerType, executer);
        }

        private void CompileCancellableCommandWithNoResult(Type commandType, Type commandHandlerType)
        {
            Type castCommandHandler = typeof(ICancellableCommandHandler<>);
            Type[] typeArgs = new[] { commandType };
            Type genericType = castCommandHandler.MakeGenericType(typeArgs);

            Type[] methodTypeArgs = new[] { commandType, typeof(CancellationToken) };
            MethodInfo methodInfo = genericType.GetRuntimeMethod("ExecuteAsync", methodTypeArgs);
            ParameterExpression handlerParameter = Expression.Parameter(typeof(ICommandHandler));
            ParameterExpression commandParameter = Expression.Parameter(typeof(ICommand));
            ParameterExpression cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken));

            var lambda = Expression.Lambda<Func<ICommandHandler, ICommand, CancellationToken, Task>>(
                Expression.Call(Expression.Convert(handlerParameter, genericType),
                    methodInfo,
                    Expression.Convert(commandParameter, commandType),
                    cancellationTokenParameter),
                handlerParameter, commandParameter, cancellationTokenParameter);
            Func<ICommandHandler, ICommand, CancellationToken, Task> executer = lambda.Compile();
            _commandHandlerExecuters.Add(commandHandlerType, executer);
        }

        private void CompileCommandWithResult(Type commandType, Type commandHandlerType, Type resultType,
            Type commandInterfaceType)
        {
            Type castCommandHandler = typeof(ICommandHandler<,>);
            Type[] typeArgs = new[] { commandType, resultType };
            Type genericType = castCommandHandler.MakeGenericType(typeArgs);


            MethodInfo methodInfo = genericType.GetRuntimeMethod("ExecuteAsync", typeArgs);
            ParameterExpression handlerParameter = Expression.Parameter(typeof(ICommandHandler));
            ParameterExpression commandParameter = Expression.Parameter(commandInterfaceType);
            ParameterExpression previousResultParameter = Expression.Parameter(resultType);

            var lambda = Expression.Lambda(
                Expression.Call(Expression.Convert(handlerParameter, genericType),
                    methodInfo,
                    Expression.Convert(commandParameter, commandType), previousResultParameter),
                handlerParameter, commandParameter, previousResultParameter);
            Delegate executer = lambda.Compile();
            _commandHandlerExecuters.Add(commandHandlerType, executer);
        }

        private void CompileCancellableCommandWithResult(Type commandType, Type commandHandlerType, Type resultType,
            Type commandInterfaceType)
        {
            Type castCommandHandler = typeof(ICancellableCommandHandler<,>);
            Type[] typeArgs = new[] { commandType, resultType };
            Type genericType = castCommandHandler.MakeGenericType(typeArgs);

            Type[] methodTypeArgs = new[] { commandType, resultType, typeof(CancellationToken) };
            MethodInfo methodInfo = genericType.GetRuntimeMethod("ExecuteAsync", methodTypeArgs);
            ParameterExpression handlerParameter = Expression.Parameter(typeof(ICommandHandler));
            ParameterExpression commandParameter = Expression.Parameter(commandInterfaceType);
            ParameterExpression previousResultParameter = Expression.Parameter(resultType);
            ParameterExpression cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken));

            var lambda = Expression.Lambda(
                Expression.Call(Expression.Convert(handlerParameter, genericType),
                    methodInfo,
                    Expression.Convert(commandParameter, commandType),
                    previousResultParameter,
                    cancellationTokenParameter),
                handlerParameter, commandParameter, previousResultParameter, cancellationTokenParameter);
            Delegate executer = lambda.Compile();
            _commandHandlerExecuters.Add(commandHandlerType, executer);
        }
    }
}
