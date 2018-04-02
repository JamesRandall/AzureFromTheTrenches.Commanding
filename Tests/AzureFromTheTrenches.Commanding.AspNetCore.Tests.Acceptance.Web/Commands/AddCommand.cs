using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands
{
    public class AddCommand : ICommand<int>
    {
        public IReadOnlyCollection<int> Numbers { get; set; }
    }
}
