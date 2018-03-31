using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.AspNetCore.Implementation;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Templates
{
    public abstract class ControllerTemplate
    {
        // this will map to @Model (property name)
        public ControllerDefinition Model => new ControllerDefinition();

        public void WriteLiteral(string literal)
        {
            // replace that by a text writer for example
            Console.Write(literal);
        }

        public void Write(object obj)
        {
            // replace that by a text writer for example
            Console.Write(obj);
        }

        public virtual Task ExecuteAsync()
        {
            return Task.CompletedTask;
        }

    }
}
