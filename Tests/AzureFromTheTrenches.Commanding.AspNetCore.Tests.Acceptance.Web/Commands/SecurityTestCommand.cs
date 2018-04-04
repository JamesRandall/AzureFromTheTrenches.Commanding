using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands
{
    public class NestedSecureData
    {
        [SecurityProperty]
        public string MoreSensitiveData { get; set; }
    }

    public class SecurityTestCommand : ICommand
    {
        [SecurityProperty]
        [BindNever]
        public string SensitiveData { get; set; }

        public string AnotherPieceOfData { get; set; }

        public NestedSecureData More { get; set; }
    }
}
