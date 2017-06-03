using Newtonsoft.Json;

namespace AccidentalFish.Commanding.AzureStorage.Implementation
{
    class AzureStorageQueueSerializer : IAzureStorageQueueSerializer
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
