// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BitPay.Converters;

public class DateStringConverter : DateTimeConverterBase
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        var date = ((DateTime)value).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
        writer.WriteRawValue($"\"{date}\"");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        JsonSerializer serializer)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        return (DateTime?)reader.Value;
    }
}