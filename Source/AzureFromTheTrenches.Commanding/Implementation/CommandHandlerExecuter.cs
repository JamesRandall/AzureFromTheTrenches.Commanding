using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal class CommandHandlerExecuter : ICommandHandlerExecuter
    {
        private readonly ConcurrentDictionary<Type, Delegate> _commandHandlerExecuters = 
            new ConcurrentDictionary<Type, Delegate>();

        public async Task<TResult> ExecuteAsync<TResult>(ICommandHandler handler, ICommand<TResult> command, TResult previousResult)
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
                await ExecuteActorForCommandWithNoResult(handler, wrappedCommand);
                return default(TResult);
            }
            return await ExecuteActorForCommand(handler, command, previousResult);
        }

        private async Task<TResult> ExecuteActorForCommand<TResult>(ICommandHandler handler, ICommand<TResult> command, TResult previousResult)
        {
            Delegate dlg = _commandHandlerExecuters.GetOrAdd(handler.GetType(), (handlerType) =>
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
            });

            Func<ICommandHandler, ICommand<TResult>, TResult, Task<TResult>> func =
                (Func<ICommandHandler, ICommand<TResult>, TResult, Task<TResult>>) dlg;
            return await func(handler, command, previousResult);
        }

        private async Task ExecuteActorForCommandWithNoResult(ICommandHandler handler, NoResultCommandWrapper wrappedCommand)
        {
            Delegate dlg = _commandHandlerExecuters.GetOrAdd(handler.GetType(), (handlerType) =>
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
            });
            Func<ICommandHandler, ICommand, Task> func = (Func<ICommandHandler, ICommand, Task>) dlg;
            await func(handler, wrappedCommand.Command);            
        }
    }
}
