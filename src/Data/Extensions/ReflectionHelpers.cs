using System.Collections.Concurrent;
using System.Reflection;

namespace FunnyExperience.Server.Data.Extensions;

public static class ReflectionHelpers
{
    public static readonly ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> PropertyInfoDictionary = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();

    public static IEnumerable<PropertyInfo> GetPropertiesFor<T>() where T : class, new() => GetPropertiesFor(typeof(T));
		
    public static IEnumerable<PropertyInfo> GetPropertiesFor<T>(this T type) where T : Type
    {
        if (PropertyInfoDictionary.TryGetValue(type, out var entries))
        {
            return entries;
        }

        entries = type.GetProperties().ToList();
        PropertyInfoDictionary.TryAdd(type, entries);

        return entries;
    }


}