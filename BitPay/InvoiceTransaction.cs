using Newtonsoft.Json;

namespace BitPayAPI
{
    /// <summary>
    /// Provides information about a single invoice transaction.
    /// </summary>
    public class InvoiceTransaction
    {
        [JsonProperty(PropertyName = "txid")]
        public string Txid { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; }
    }
}
