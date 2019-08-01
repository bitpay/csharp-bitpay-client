using Newtonsoft.Json;

namespace BitPaySDK.Models.Invoice
{
    /// <summary>
    ///     Provides information about a single invoice transaction.
    /// </summary>
    public class InvoiceTransaction
    {
        private dynamic _exchangeRates;
        
        [JsonProperty(PropertyName = "amount")]
        public double Amount { get; set; }

        [JsonProperty(PropertyName = "confirmations")]
        public string Confirmations { get; set; }

        [JsonProperty(PropertyName = "receivedTime")]
        public string ReceivedTime { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "txid")]
        public string Txid { get; set; }
        
        [JsonProperty(PropertyName = "refundAmount")]
        public double RefundAmount { get; set; }

        [JsonProperty(PropertyName = "time")]
        public string Time { get; set; }

        [JsonProperty(PropertyName = "exRates")]
        public dynamic ExchangeRates {
            get => _exchangeRates;
            set => _exchangeRates = JsonConvert.DeserializeObject(value.ToString(Formatting.None));
        }

        [JsonProperty(PropertyName = "outputIndex")]
        public int OutputIndex { get; set; }
    }
}