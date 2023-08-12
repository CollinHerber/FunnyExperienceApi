using System.Collections.Concurrent;

namespace FunnyExperience.Server.Data.Extensions;

public static class TypeExtensions
{
    public static readonly ConcurrentDictionary<Type, object> DefaultValueDictionary = new ConcurrentDictionary<Type, object>();

    public static bool IsDefaultValue(this Type type, object value)
    {
        if (DefaultValueDictionary.TryGetValue(type, out var defaultValue))
        {
            return value?.Equals(defaultValue) != false;
        }

        if (type == null) return false;

        defaultValue = type.IsPrimitive ? Activator.CreateInstance(type) : null;
        DefaultValueDictionary.TryAdd(type, defaultValue);

        return value?.Equals(defaultValue) != false;
    }
}