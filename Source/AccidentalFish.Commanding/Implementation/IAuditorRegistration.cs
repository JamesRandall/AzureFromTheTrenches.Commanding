namespace AccidentalFish.Commanding.Implementation
{
    internal interface IAuditorRegistration
    {
        void RegisterAuditor<TAuditorImpl>() where TAuditorImpl : ICommandAuditor;
    }
}
