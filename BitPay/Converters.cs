using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace BitPayAPI
{
    class Converters
    {

        public class DateStringConverter : DateTimeConverterBase
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteRawValue("\"" + ((DateTime)value).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz") + "\"");
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.Value == null) { return null; }
                return (DateTime)reader.Value;
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

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                // When the server returns null write a 0.
                if (reader.Value == null)
                {
                    return 0.0;
                }
                else
                {
                    return Convert.ToDouble(reader.Value);
                }
            }
        }    

    }
}
