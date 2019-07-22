using Newtonsoft.Json;

namespace BitPaySDK.Models.Invoice
{
    /// <summary>
    ///     Provides information about a single invoice transaction.
    /// </summary>
    public class InvoiceTransaction
    {
        [JsonProperty(PropertyName = "amount")]
        public double Amount { get; set; }

        [JsonProperty(PropertyName = "confirmations")]
        public string Confirmations { get; set; }

        [JsonProperty(PropertyName = "receivedTime")]
        public string ReceivedTime { get; set; }

        [JsonProperty(PropertyName = "time")] public string Time { get; set; }

        [JsonProperty(PropertyName = "type")] public string Type { get; set; }
    }
}