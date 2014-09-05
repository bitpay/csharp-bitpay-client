using Newtonsoft.Json;
using System;

namespace BitPayAPI
{
    /// <summary>
    /// Provides an interface to a single exchange rate.
    /// </summary>
    public class Rate
    {
        public Rate() {}

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "rate")]
        public decimal Value { get; set; }
    }
}
