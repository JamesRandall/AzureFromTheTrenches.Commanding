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
    internal class PipelineAwareCommandHandlerExecuter : IPipelineAwareCommandHandlerExecuter
    {
        private readonly ConcurrentDictionary<Type, Delegate> _commandHandlerExecuters =
            new ConcurrentDictionary<Type, Delegate>();

        public async Task<PipelineAwareCommandHandlerResult<TResult>> ExecuteAsync<TResult>(IPipelineAwareCommandHandler handler, ICommand<TResult> command, TResult previousResult, CancellationToken cancellationToken)
        {
            // we compile this expression to enable command actors to be written with a strongly typed
            // command type syntax e.g.:
            //  class MyCommandActor : IPipelineAwareCommandHandler<MyCommand, MyResult>
            // Without this command actors would need to be of the form:
            //  class MyCommandActor : IPipelineAwareCommandHandler<ICommand<MyResult>>
            // Which would lead to lots of casting inside actors. During registration of commands we can guarantee
            // type safety.

            Delegate dlg = _commandHandlerExecuters.GetOrAdd(handler.GetType(), (handlerType) =>
            {
                if (handler is ICancellablePipelineAwareCommandHandler)
                {
                    Func<IPipelineAwareCommandHandler, ICommand<TResult>, TResult, CancellationToken, Task<PipelineAwareCommandHandlerResult<TResult>>> executer = CompileCancellableCommandHandlerExecuter(command);
                    return executer;
                }
                else
                {
                    Func<IPipelineAwareCommandHandler, ICommand<TResult>, TResult, Task<PipelineAwareCommandHandlerResult<TResult>>> executer = CompileCommandHandlerExecuter(command);
                    return executer;
                }                
            });

            if (handler is ICancellablePipelineAwareCommandHandler)
            {
                Func<IPipelineAwareCommandHandler, ICommand<TResult>, TResult, CancellationToken, Task<PipelineAwareCommandHandlerResult<TResult>>> func = (Func<IPipelineAwareCommandHandler, ICommand<TResult>, TResult, CancellationToken, Task<PipelineAwareCommandHandlerResult<TResult>>>)dlg;
                return await func(handler, command, previousResult, cancellationToken);
            }
            else
            {
                Func<IPipelineAwareCommandHandler, ICommand<TResult>, TResult, Task<PipelineAwareCommandHandlerResult<TResult>>> func = (Func<IPipelineAwareCommandHandler, ICommand<TResult>, TResult, Task<PipelineAwareCommandHandlerResult<TResult>>>)dlg;
                return await func(handler, command, previousResult);
            }            
        }

        private static Func<IPipelineAwareCommandHandler, ICommand<TResult>, TResult, Task<PipelineAwareCommandHandlerResult<TResult>>> CompileCommandHandlerExecuter<TResult>(ICommand<TResult> command)
        {
            Type castCommandHandler = typeof(IPipelineAwareCommandHandler<,>);
            Type[] typeArgs = new[] {command.GetType(), typeof(TResult)};
            Type genericType = castCommandHandler.MakeGenericType(typeArgs);

            MethodInfo methodInfo = genericType.GetRuntimeMethod("ExecuteAsync", typeArgs);
            ParameterExpression handlerParameter = Expression.Parameter(typeof(IPipelineAwareCommandHandler));
            ParameterExpression commandParameter = Expression.Parameter(typeof(ICommand<TResult>));
            ParameterExpression previousResultParameter = Expression.Parameter(typeof(TResult));

            var lambda =
                Expression.Lambda<Func<IPipelineAwareCommandHandler, ICommand<TResult>, TResult, Task<PipelineAwareCommandHandlerResult<TResult>>>>(
                    Expression.Call(Expression.Convert(handlerParameter, genericType),
                        methodInfo,
                        Expression.Convert(commandParameter, command.GetType()), previousResultParameter),
                    handlerParameter, commandParameter, previousResultParameter);
            Func<IPipelineAwareCommandHandler, ICommand<TResult>, TResult, Task<PipelineAwareCommandHandlerResult<TResult>>> executer =
                lambda.Compile();
            return executer;
        }

        private static Func<IPipelineAwareCommandHandler, ICommand<TResult>, TResult, CancellationToken, Task<PipelineAwareCommandHandlerResult<TResult>>> CompileCancellableCommandHandlerExecuter<TResult>(ICommand<TResult> command)
        {
            Type castCommandHandler = typeof(ICancellablePipelineAwareCommandHandler<,>);
            Type[] typeArgs = new[] { command.GetType(), typeof(TResult) };
            Type genericType = castCommandHandler.MakeGenericType(typeArgs);

            Type[] methodTypeArgs = new[] { command.GetType(), typeof(TResult), typeof(CancellationToken) };
            MethodInfo methodInfo = genericType.GetRuntimeMethod("ExecuteAsync", methodTypeArgs);
            ParameterExpression handlerParameter = Expression.Parameter(typeof(IPipelineAwareCommandHandler));
            ParameterExpression commandParameter = Expression.Parameter(typeof(ICommand<TResult>));
            ParameterExpression previousResultParameter = Expression.Parameter(typeof(TResult));
            ParameterExpression cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken));

            var lambda =
                Expression.Lambda<Func<IPipelineAwareCommandHandler, ICommand<TResult>, TResult, CancellationToken, Task<PipelineAwareCommandHandlerResult<TResult>>>>(
                    Expression.Call(Expression.Convert(handlerParameter, genericType),
                        methodInfo,
                        Expression.Convert(commandParameter, command.GetType()),
                        previousResultParameter,
                        cancellationTokenParameter),
                    handlerParameter, commandParameter, previousResultParameter, cancellationTokenParameter);
            Func<IPipelineAwareCommandHandler, ICommand<TResult>, TResult, CancellationToken, Task<PipelineAwareCommandHandlerResult<TResult>>> executer =
                lambda.Compile();
            return executer;
        }
    }
}
