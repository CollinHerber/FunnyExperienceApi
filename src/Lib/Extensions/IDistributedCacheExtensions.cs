using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace FunnyExperience.Server.Lib.Extensions;

public static class IDistributedCacheExtensions
{

    /// <summary>
    /// You pass a Func and this will try to either get the result or run the Func e.g. () => Task.Run
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cache"></param>
    /// <param name="key"></param>
    /// <param name="task"></param>
    /// <returns></returns>
    public static async Task<T> GetOrSetAsAsync<T>(this IDistributedCache cache, string key, Func<Task<T>> task)
    {
        var bytes = await cache.GetAsync(key);
        if (bytes != null) { return JsonSerializer.Deserialize<T>(bytes); }

        var result = await task();
        await cache.SetAsync(key, JsonSerializer.SerializeToUtf8Bytes(result));
        return result;
    }

    /// <summary>
    /// You pass a Func and this will try to either get the result or run the Func e.g. () => Task.Run
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cache"></param>
    /// <param name="key"></param>
    /// <param name="hours"></param>
    /// <param name="task"></param>
    /// <returns></returns>
    public static async Task<T> GetOrSetAsWithHourlyExpirationAsync<T>(this IDistributedCache cache, string key, int hours, Func<Task<T>> task)
    {
        var bytes = await cache.GetAsync(key);
        if (bytes != null) { return JsonSerializer.Deserialize<T>(bytes); }

        var result = await task();
        await cache.SetAsync(key, JsonSerializer.SerializeToUtf8Bytes(result), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(hours)
        });
        return result;
    }

    /// <summary>
    /// You pass a Func and this will try to either get the result or run the Func e.g. () => Task.Run
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cache"></param>
    /// <param name="key"></param>
    /// <param name="minutes"></param>
    /// <param name="task"></param>
    /// <returns></returns>
    public static async Task<T> GetOrSetAsWithMinuteExpirationAsync<T>(this IDistributedCache cache, string key, int minutes, Func<Task<T>> task)
    {
        var bytes = await cache.GetAsync(key);
        if (bytes != null) { return JsonSerializer.Deserialize<T>(bytes); }

        var result = await task();
        await cache.SetAsync(key, JsonSerializer.SerializeToUtf8Bytes(result), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutes)
        });
        return result;
    }
    
    /// <summary>
    /// You pass a Func and this will try to either get the result or run the Func e.g. () => Task.Run
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cache"></param>
    /// <param name="key"></param>
    /// <param name="minutes"></param>
    /// <param name="task"></param>
    /// <returns></returns>
    public async static Task<T> GetOrSetNullableAsWithMinuteExpirationAsync<T>(this IDistributedCache cache, string key, int minutes, Func<Task<T>> task)
    {
        var bytes = await cache.GetAsync(key);
        if (bytes != null) { return JsonSerializer.Deserialize<T>(bytes); }

        var result = await task();
        if (result != null)
        {
            await cache.SetAsync(key, JsonSerializer.SerializeToUtf8Bytes(result), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutes)
            });   
        }
        return result;
    }
    
    /// <summary>
    /// You pass a Func and this will try to either get the result or run the Func e.g. () => Task.Run
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cache"></param>
    /// <param name="key"></param>
    /// <param name="hours"></param>
    /// <param name="task"></param>
    /// <returns></returns>
    public async static Task<T> GetOrSetNullableAsWithHourlyExpirationAsync<T>(this IDistributedCache cache, string key, int hours, Func<Task<T>> task)
    {
        var bytes = await cache.GetAsync(key);
        if (bytes != null) { return JsonSerializer.Deserialize<T>(bytes); }

        var result = await task();
        if (result != null)
        {
            await cache.SetAsync(key, JsonSerializer.SerializeToUtf8Bytes(result), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(hours)
            });   
        }
        return result;
    }

    /// <summary>
    /// Calls a func using a Wrapper task in case the func isn't async
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cache"></param>
    /// <param name="key"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static Task<T> GetOrSetAsAsync<T>(this IDistributedCache cache, string key, Func<T> func)
    {
        return cache.GetOrSetAsAsync(key, () => Task.FromResult(func()));
    }
}