using System;
using System.Threading.Tasks;

namespace AzureFromTheTrenches.Commanding.Cache
{
    public interface ICacheAdapter
    {
        Task Set(string key, object value, TimeSpan lifeTime);
        Task Set(string key, object value, DateTime expiresAt);
        Task<T> Get<T>(string key);
    }
}
