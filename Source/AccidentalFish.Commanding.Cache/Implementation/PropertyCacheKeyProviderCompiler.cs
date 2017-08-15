using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AccidentalFish.Commanding.Cache.Implementation
{
    internal class PropertyCacheKeyProviderCompiler : IPropertyCacheKeyProviderCompiler
    {
        public Func<TCommand, string> Compile<TCommand>()
        {
            Func<TCommand, string> compiledFunc = null;
            var commandParameter = Expression.Parameter(typeof(TCommand));
            MethodInfo toStringMethodInfo = typeof(object).GetTypeInfo().GetDeclaredMethod("ToString");
            MethodInfo gethashcodeMethodInfo = typeof(object).GetTypeInfo().GetDeclaredMethod("GetHashCode");
            PropertyInfo[] properties = typeof(TCommand).GetTypeInfo().DeclaredProperties.OrderBy(x => x.Name).ToArray();
            Expression[] concatParameters = new Expression[properties.Length*3 + 1];
            concatParameters[0] = Expression.Constant(typeof(TCommand).Name);
            for (int propertyIndex = 0; propertyIndex < properties.Length; propertyIndex++)
            {
                concatParameters[propertyIndex * 3 + 1] = Expression.Constant("|");
                concatParameters[propertyIndex * 3 + 2] = Expression.Constant(properties[propertyIndex].Name + ":");
                concatParameters[propertyIndex * 3 + 3] = Expression.Call(Expression.Property(commandParameter, properties[propertyIndex]), toStringMethodInfo);
                
            }

            MethodInfo concatMethodInfo = typeof(string).GetTypeInfo().GetDeclaredMethods("Concat").Single(x => x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == typeof(IEnumerable<string>));
            
            
            Expression concatCall = Expression.Call(null, concatMethodInfo, Expression.NewArrayInit(typeof(string), concatParameters));
            Expression gethashcodeCall = Expression.Call(Expression.Call(concatCall, gethashcodeMethodInfo), toStringMethodInfo);
            var lambda = Expression.Lambda<Func<TCommand, string>>(gethashcodeCall, commandParameter);
            compiledFunc = lambda.Compile();
            
            return compiledFunc;
        }
    }
}
