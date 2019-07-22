using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BitPaySDK
{
    public class Converters
    {
        public class DateStringConverter : DateTimeConverterBase
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteRawValue("\"" + ((DateTime) value).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz") + "\"");
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                return (DateTime?) reader.Value;
            }
        }

        public class BtcValueConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return true;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                // When the server returns null write a 0.
                return reader.Value == null ? 0.0 : Convert.ToDouble(reader.Value);
            }
        }
    }
}