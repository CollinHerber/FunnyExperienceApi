using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace FunnyExperience.Server.Lib.Extensions;

public static class HttpContentExtensions
{
    public static async Task<T> DeserializeAsync<T>(this HttpContent httpContent, JsonSerializer serializer = null) where T : class
    {
        serializer ??= JsonSerializer.CreateDefault();
        using var stream = await httpContent.ReadAsStreamAsync();
        return await stream.DeserializeAsync<T>(serializer);
    }

    public static async Task<T> DeserializeAsync<T>(this HttpContent httpContent, JsonSerializerSettings jsonSerializerSettings = null) where T : class
    {
        var serializer = jsonSerializerSettings == null ? null : JsonSerializer.Create(jsonSerializerSettings);
        using var stream = await httpContent.ReadAsStreamAsync();
        return await stream.DeserializeAsync<T>(serializer);
    }
}