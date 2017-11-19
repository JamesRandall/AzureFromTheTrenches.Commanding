using System.Threading.Tasks;

namespace AzureFromTheTrenches.Commanding.AzureEventHub.Implementation
{
    interface IEventHubClient
    {
        Task SendAsync(string text, string partitionKey);
        Task SendAsync(string text);
    }
}
