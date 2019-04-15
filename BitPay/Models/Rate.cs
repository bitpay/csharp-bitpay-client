using Newtonsoft.Json;

namespace BitPayAPI.Models
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