using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Tests.Performance.Console.Model
{
    class SimpleCommand : ICommand<SimpleResult>
    {
    }

    class SimpleCommandNoResult : ICommand
    {

    }
}
