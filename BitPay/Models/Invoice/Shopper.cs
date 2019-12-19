using Newtonsoft.Json;

namespace BitPaySDK.Models.Invoice
{
    public class Shopper
    {
        [JsonProperty(PropertyName = "user")]
        public string User { get; set; }
    }
}
