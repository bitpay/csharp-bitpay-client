// Copyright (c) 2019 BitPay.
// All rights reserved.

using Newtonsoft.Json;

namespace BitPay.Models.Rate
{
    /// <summary>
    ///     Provides an interface to a single exchange rate.
    /// </summary>
    public class Rate
    {
        [JsonProperty(PropertyName = "name")] public string Name { get; set; }

        [JsonProperty(PropertyName = "code")] public string Code { get; set; }

        [JsonProperty(PropertyName = "rate")] public decimal Value { get; set; }
    }
}