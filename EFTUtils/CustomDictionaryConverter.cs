using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable UnusedType.Global

namespace EFTUtils
{
    public class CustomDictionaryConverter<TKey, TValue> : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var iDictionary = value as IDictionary<TKey, TValue> ??
                              throw new Exception(
                                  $"{value.GetType()} can't converter to IDictionary<{typeof(TKey)}, {typeof(TValue)}>");

            writer.WriteStartArray();

            foreach (var kvp in iDictionary)
            {
                writer.WriteStartArray();

                serializer.Serialize(writer, kvp.Key);
                serializer.Serialize(writer, kvp.Value);

                writer.WriteEndArray();
            }

            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (!CanConvert(objectType))
            {
                throw new Exception($"{objectType} can't converter to IDictionary<{typeof(TKey)}, {typeof(TValue)}>");
            }

            return JToken.Load(reader).ToDictionary(x => x[0].ToObject<TKey>(), x => x[1].ToObject<TValue>());
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IDictionary<TKey, TValue>).IsAssignableFrom(objectType);
        }
    }
}