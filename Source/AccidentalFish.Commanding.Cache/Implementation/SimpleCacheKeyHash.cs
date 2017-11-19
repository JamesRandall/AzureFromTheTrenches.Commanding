namespace AzureFromTheTrenches.Commanding.Cache.Implementation
{
    internal class SimpleCacheKeyHash : ICacheKeyHash
    {
        public string GetHash(string key)
        {
            unchecked
            {
                int hash = 23;
                foreach (char c in key)
                {
                    hash = hash * 31 + c;
                }
                return hash.ToString();
            }

        }
    }
}
