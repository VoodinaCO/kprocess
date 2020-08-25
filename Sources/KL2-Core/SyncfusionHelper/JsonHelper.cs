using Newtonsoft.Json;
using System;

namespace SyncfusionHelper
{
    public static class JsonHelper
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Converters = new JsonConverter[]
                {
                    new EnumValueConverter()
                }
        };
    }

    public class EnumValueConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            serializer.Serialize(writer, Enum.GetName(value.GetType(), value).ToLower());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead =>
            false;

        public override bool CanConvert(Type objectType) =>
            objectType.IsEnum;
    }
}
