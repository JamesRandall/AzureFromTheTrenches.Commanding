namespace AzureFromTheTrenches.Commanding.Http
{
    public interface IHttpCommandSerializer
    {
        /// <summary>
        /// Serialize the given object to a string
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <returns>A serialized represtentation of the object</returns>
        string Serialize(object obj);

        /// <summary>
        /// Deserialize an object from a string
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="serializedRepresentation">The serialized representation of the object</param>
        /// <returns>Deserialized object</returns>
        T Deserialize<T>(string serializedRepresentation);

        /// <summary>
        /// The media type to supply as a content type and response type on the Http request. For example application/json
        /// </summary>
        string MimeType { get; }
    }
}
