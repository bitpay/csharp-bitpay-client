using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BitPayAPI
{
    /// <summary>
    /// Provides information about a single invoice transaction.
    /// </summary>
    public class InvoiceTransaction
    {
        public InvoiceTransaction() { }

        [JsonProperty(PropertyName = "txid")]
        public string Txid { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public double Amount { get; set; }
    }
}
