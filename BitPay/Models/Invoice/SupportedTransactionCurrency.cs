// Copyright (c) 2019 BitPay.
// All rights reserved.

using Newtonsoft.Json;

namespace BitPay.Models.Invoice
{
    public class SupportedTransactionCurrency
    {
        [JsonProperty(PropertyName = "enabled")]
        public bool Enabled { get; set; }
        
        [JsonProperty(PropertyName = "reason")]
        public string? Reason { get; set; }
        
        public SupportedTransactionCurrency(bool enabled)
        {
            Enabled = enabled;
        }
    }
}