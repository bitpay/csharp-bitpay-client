using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BitPayAPI
{
    public class LedgerEntry
    {
        public LedgerEntry() {}

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty(PropertyName = "scale")]
        public string Scale { get; set; }

        [JsonProperty(PropertyName = "txType")]
        public string TxType { get; set; }

        [JsonProperty(PropertyName = "sale")]
        public string Sale { get; set; }

        [JsonProperty(PropertyName = "exRates")]
        public Dictionary<string, string> ExRates { get; set; }

        [JsonProperty(PropertyName = "buyer")]
        public Buyer Buyer { get; set; }

        [JsonProperty(PropertyName = "invoiceId")]
        public string InvoiceId { get; set; }

        [JsonProperty(PropertyName = "sourceType")]
        public string SourceType { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<string, string> CustomerData { get; set; }

        [JsonProperty(PropertyName = "invoiceAmount")]
        public double InvoiceAmount { get; set; }

        [JsonProperty(PropertyName = "invoiceCurrency")]
        public string invoiceCurrency { get; set; }
    }
}
