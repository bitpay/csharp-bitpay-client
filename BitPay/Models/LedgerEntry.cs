using System.Collections.Generic;
using Newtonsoft.Json;

namespace BitPayAPI.Models
{
    public class LedgerEntry
    {
        [JsonProperty(PropertyName = "code")] public string Code { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty(PropertyName = "scale")] public string Scale { get; set; }

        [JsonProperty(PropertyName = "txType")]
        public string TxType { get; set; }

        [JsonProperty(PropertyName = "sale")] public string Sale { get; set; }

        [JsonProperty(PropertyName = "buyer")] public Buyer Buyer { get; set; }

        [JsonProperty(PropertyName = "invoiceId")]
        public string InvoiceId { get; set; }

        [JsonProperty(PropertyName = "sourceType")]
        public string SourceType { get; set; }

        [JsonIgnore] public Dictionary<string, string> CustomerData { get; set; }

        [JsonProperty(PropertyName = "invoiceAmount")]
        public double InvoiceAmount { get; set; }

        [JsonProperty(PropertyName = "invoiceCurrency")]
        public string InvoiceCurrency { get; set; }

        [JsonProperty(PropertyName = "transactionCurrency")]
        public string TransactionCurrency { get; set; }
    }
}