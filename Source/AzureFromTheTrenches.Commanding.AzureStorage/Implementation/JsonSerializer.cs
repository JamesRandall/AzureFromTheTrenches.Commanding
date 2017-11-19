using Newtonsoft.Json;

namespace AzureFromTheTrenches.Commanding.AzureStorage.Implementation
{
    internal class JsonSerializer : IAzureStorageQueueSerializer
    {
        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public T Deserialize<T>(string serializedRepresentation)
        {
            return JsonConvert.DeserializeObject<T>(serializedRepresentation);
        }
    }
}
