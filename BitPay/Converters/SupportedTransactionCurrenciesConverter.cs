// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Collections.Generic;

using BitPay.Models.Invoice;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BitPay.Converters
{
    public class SupportedTransactionCurrenciesConverter : JsonConverter<SupportedTransactionCurrencies>
    {
        public override SupportedTransactionCurrencies? ReadJson(
            JsonReader reader,
            Type objectType,
            SupportedTransactionCurrencies? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
        )
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new JsonSerializationException($"Unexpected token type: {reader.TokenType}");
            }

            Dictionary<string, SupportedTransactionCurrency> additionalProperties = new();
            
            var supportedTransactionCurrencies = existingValue ?? new SupportedTransactionCurrencies(additionalProperties);


            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType != JsonToken.PropertyName)
                {
                    throw new JsonSerializationException($"Unexpected token type: {reader.TokenType}");
                }

                string key = (string)reader.Value!;

                reader.Read();

                SupportedTransactionCurrency? supportedTransactionCurrency =
                    serializer.Deserialize<SupportedTransactionCurrency>(reader);

                if (supportedTransactionCurrency != null)
                {
                    additionalProperties.Add(key, supportedTransactionCurrency);
                }
            }

            supportedTransactionCurrencies.SupportedCurrencies = additionalProperties;

            return supportedTransactionCurrencies;
        }

        public override void WriteJson(
            JsonWriter writer,
            SupportedTransactionCurrencies? value,
            JsonSerializer serializer
        )
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            
            JObject jo = new JObject();

            // writer.WriteStartObject();
            foreach(KeyValuePair<string, SupportedTransactionCurrency> entry in value.SupportedCurrencies)
            {
                jo.Add(entry.Key, entry.Value.Enabled);
            }
            
            jo.WriteTo(writer);
        }
    }
}