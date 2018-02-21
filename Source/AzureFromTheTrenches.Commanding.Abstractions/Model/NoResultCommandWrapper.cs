namespace AzureFromTheTrenches.Commanding.Abstractions.Model
{
    public class NoResultCommandWrapper : ICommand<NoResult>
    {
        public NoResultCommandWrapper(ICommand command)
        {
            Command = command;
        }

        public ICommand Command { get; }        
    }
}
