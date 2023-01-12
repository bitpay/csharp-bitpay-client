using Newtonsoft.Json;

namespace BitPay.Models.Invoice
{
    public class Shopper
    {
        [JsonProperty(PropertyName = "user")]
        public string User { get; set; }
    }
}
