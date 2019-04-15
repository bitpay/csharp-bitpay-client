using Newtonsoft.Json;

namespace BitPayAPI.Models
{
    /// <summary>
    ///     Provides an interface to a buyer.
    /// </summary>
    public class Buyer
    {
        [JsonProperty(PropertyName = "name")] public string Name { get; set; }

        [JsonProperty(PropertyName = "address1")]
        public string Address1 { get; set; }

        [JsonProperty(PropertyName = "address2")]
        public string Address2 { get; set; }

        [JsonProperty(PropertyName = "city")] public string City { get; set; }

        [JsonProperty(PropertyName = "state")] public string State { get; set; }

        [JsonProperty(PropertyName = "zip")] public string zip { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string country { get; set; }

        [JsonProperty(PropertyName = "email")] public string email { get; set; }

        [JsonProperty(PropertyName = "phone")] public string phone { get; set; }
    }
}