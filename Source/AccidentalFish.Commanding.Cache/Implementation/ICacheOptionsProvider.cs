namespace AzureFromTheTrenches.Commanding.Cache.Implementation
{
    interface ICacheOptionsProvider
    {
        CacheOptions Get<T>(T command);
    }
}
