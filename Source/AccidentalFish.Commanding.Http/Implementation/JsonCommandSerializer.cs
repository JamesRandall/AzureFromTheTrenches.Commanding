using Newtonsoft.Json;

namespace AccidentalFish.Commanding.Http.Implementation
{
    internal class JsonCommandSerializer : IHttpCommandSerializer
    {
        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public T Deserialize<T>(string serializedRepresentation)
        {
            return JsonConvert.DeserializeObject<T>(serializedRepresentation);
        }

        public string MimeType => "application/json";
    }
}
