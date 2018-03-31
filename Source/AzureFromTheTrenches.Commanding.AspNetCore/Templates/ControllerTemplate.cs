using System.Text;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Templates
{
    public abstract class ControllerTemplate
    {
        protected ControllerTemplate()
        {
            Output = new StringBuilder();
        }

        public ControllerDefinition Model { get; set; }

        public StringBuilder Output { get; }

        public void WriteLiteral(string literal)
        {
            Output.Append(literal);
        }

        public void Write(object obj)
        {
            Output.Append(obj);
        }

        public virtual Task ExecuteAsync()
        {
            return Task.CompletedTask;
        }

    }
}
