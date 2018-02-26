using System.Threading.Tasks;

namespace AzureFromTheTrenches.Commanding.Tests.Performance.Console.Model
{
    internal class SimpleClass
    {
        public Task<SimpleResult> DoSomething()
        {
            return Task.FromResult<SimpleResult>(null);
        }
    }
}
