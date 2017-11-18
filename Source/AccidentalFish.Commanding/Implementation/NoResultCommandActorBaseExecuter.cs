using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions;

namespace AccidentalFish.Commanding.Implementation
{
    internal class NoResultCommandActorBaseExecuter : INoResultCommandActorBaseExecuter
    {
        private readonly ConcurrentDictionary<Type, Func<object, object, Task>> _compiledExecuters = new ConcurrentDictionary<Type, Func<object, object, Task>>();

        public async Task ExecuteAsync(object actorInstance, object command)
        {
            if (!_compiledExecuters.TryGetValue(actorInstance.GetType(), out Func<object, object, Task> executer))
            {
                TypeInfo typeInfo = actorInstance.GetType().GetTypeInfo();
                MethodInfo methodInfo = typeInfo.GetDeclaredMethod("ExecuteAsync");
                ParameterInfo[] parameterInfos = methodInfo?.GetParameters();
                if (parameterInfos?.Length == 2 && parameterInfos[0].ParameterType == command.GetType())
                {
                    ParameterExpression instanceParameter = Expression.Parameter(typeof(object));
                    ParameterExpression commandParameter = Expression.Parameter(typeof(object));
                    ConstantExpression previousResultConstant = Expression.Constant(Activator.CreateInstance(parameterInfos[1].ParameterType));
                    Expression body =
                        Expression.Call(
                            Expression.Convert(instanceParameter, typeInfo.AsType()),
                            methodInfo,
                            Expression.Convert(commandParameter, command.GetType()),
                            previousResultConstant);

                    executer = Expression.Lambda<Func<object, object, Task>>(body, instanceParameter, commandParameter).Compile();
                    _compiledExecuters[actorInstance.GetType()] = executer;
                }
                else
                {
                    throw new UnableToExecuteActorException("Malformed actor");
                }
            }

            await executer(actorInstance, command);
        }
    }
}
