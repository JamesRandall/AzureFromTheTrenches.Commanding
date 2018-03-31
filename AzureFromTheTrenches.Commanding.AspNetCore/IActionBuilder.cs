using System.Net.Http;

namespace AzureFromTheTrenches.Commanding.AspNetCore
{
    public interface IActionBuilder
    {
        IActionBuilder Action<TCommand, TResult>(HttpMethod method, string action = null);
    }
}
