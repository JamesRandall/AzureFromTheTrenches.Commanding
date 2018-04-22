using System;
using System.Linq;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Model
{
    public abstract class AbstractFunctionDefinition
    {
        public string Namespace { get; set; }

        public string Name { get; set; }

        public Type CommandType { get; set; }

        public string CommandTypeName => CommandType.FullName;

        public Type CommandResultType
        {
            get
            {
                Type commandInterface = typeof(ICommand);
                Type genericCommandInterface = CommandType.GetInterfaces()
                    .SingleOrDefault(x => x.IsGenericType && commandInterface.IsAssignableFrom(x));

                if (genericCommandInterface != null)
                {
                    return genericCommandInterface.GenericTypeArguments[0];
                }

                return null;
            }
        }

        public string CommandResultTypeName => CommandResultType?.FullName;

    }
}
