// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;

using Newtonsoft.Json;

namespace BitPay.Converters
{
    public class BtcValueConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer
        )
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            // When the server returns null write a 0.
            return reader.Value == null ? 0.0 : Convert.ToDouble(reader.Value);
        }
    }
}