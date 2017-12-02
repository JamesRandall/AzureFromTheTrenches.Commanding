using System.Threading.Tasks;
using WindowsFormsCommanding.Commands;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace WindowsFormsCommanding.Handlers
{
    class GetTextCommandHandler : ICommandHandler<GetTextCommand, string>
    {
        public async Task<string> ExecuteAsync(GetTextCommand command, string previousResult)
        {
            // Awaiting here would, without specific management of the synchronisation context, cause a deadlock.
            // I've put this here, along with the comment, to demonstrate that the commanding framework works correctly
            // inside such environments.
            await Task.Delay(100);
            return
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        }
    }
}
