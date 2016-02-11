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

        [JsonProperty(PropertyName = "amount")]
        public double Amount { get; set; }

        [JsonProperty(PropertyName = "confirmations")]
        public string Confirmations { get; set; }

        [JsonProperty(PropertyName = "receivedTime")]
        public string ReceivedTime { get; set; }

        [JsonProperty(PropertyName = "time")]
        public string Time { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }
}
