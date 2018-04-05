using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands
{
    public class SecurityTestCommand : ICommand
    {
        [SecurityProperty]
        public string SensitiveData { get; set; }

        public string AnotherPieceOfData { get; set; }
    }
}
