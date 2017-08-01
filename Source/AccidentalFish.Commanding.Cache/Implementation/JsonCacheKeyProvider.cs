using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace AccidentalFish.Commanding.Cache.Implementation
{
    internal class JsonCacheKeyProvider : ICacheKeyProvider
    {
        // TODO: this is *absolutely* not intended as a final implementation
        public string CacheKey<T>(T command)
        {
            string json = $"{command.GetType().FullName}|{JsonConvert.SerializeObject(command)}";
            return json.GetHashCode().ToString();
        }
    }
}
