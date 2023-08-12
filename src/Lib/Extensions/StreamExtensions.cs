using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace FunnyExperience.Server.Lib.Extensions;

public static class StreamExtensions
{
    /// <summary>
    /// Deserializes a stream using default serializer if one not given
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="serializer"></param>
    /// <returns></returns>
    public static Task<T> DeserializeAsync<T>(this Stream stream, JsonSerializer serializer = null) where T : class
    {
        // Caller is responsible for closing
        return Task.Run(() =>
        {
            var sr = new StreamReader(stream);
            serializer ??= JsonSerializer.CreateDefault();
            using var jsonStream = new JsonTextReader(sr);
            return serializer.Deserialize<T>(jsonStream);
        });
    }

    /// <summary>
    /// Deserializes a stream using settings created without default options
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="jsonSerializerSettings"></param>
    /// <returns></returns>
    public static Task<T> DeserializeAsync<T>(this Stream stream, JsonSerializerSettings jsonSerializerSettings = null) where T : class
    {
        var serializer = jsonSerializerSettings == null ? null : JsonSerializer.Create(jsonSerializerSettings);
        return stream.DeserializeAsync<T>(serializer);
    }

}