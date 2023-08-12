using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FunnyExperience.Server.Models.Converters;

public class EnumFlagConverter<T> : JsonConverter where T : struct, IConvertible
{
    public override bool CanConvert(Type objectType)
    {
        return objectType.Equals(typeof(T)) || objectType.Equals(typeof(T?));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return default(T);

        var jArray = JArray.Load(reader);

        if (!jArray.HasValues)
            return default(T);

        var list = JsonConvert.DeserializeObject<List<string>>(jArray.ToString());
        if (list == null)
            return default(T);

        long toReturn = 0;
        foreach (var item in list)
        {
            T eVal;
            if (Enum.TryParse<T>(item, out eVal))
                toReturn |= Convert.ToInt64(eVal);
            else
            {
                var deVal = GetValueFromDescription(item);
                if (deVal != null)
                    toReturn |= Convert.ToInt64(deVal);
            }

        }

        return (T)Enum.ToObject(typeof(T), toReturn);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var toReturn = new List<string>();
        if (value != null)
        {
            foreach (Enum val in Enum.GetValues(typeof(T)))
            {
                if (((Enum)value).HasFlag(val) && Convert.ToInt64(value) != 0)
                    toReturn.Add(GetEnumDescription(val));
            }
        }

        serializer.Serialize(writer, toReturn);
    }

    public T? GetValueFromDescription(string description)
    {
        var type = typeof(T);
        if (!type.IsEnum) throw new InvalidOperationException();
        foreach (var field in type.GetFields())
        {
            var attribute = Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (attribute != null)
            {
                if (attribute.Description == description)
                    return (T)field.GetValue(null);
            }
            else
            {
                if (field.Name == description)
                    return (T)field.GetValue(null);
            }
        }
        return null;
    }

    public static string GetEnumDescription(Enum value)
    {
        var fi = value.GetType().GetField(value.ToString());
        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attributes != null && attributes.Length > 0)
            return attributes[0].Description;

        return value.ToString();
    }
}