// Copyright (c) 2019 BitPay.
// All rights reserved.

using System.Collections.Generic;

using Newtonsoft.Json;

namespace BitPay.Models.Invoice
{
    public class RefundInfo
    {
        [JsonProperty(PropertyName = "supportRequest")]
        public string? SupportRequest { get; set; }
        
        [JsonProperty(PropertyName = "currency")]
        public string? Currency { get; set; }
        
        [JsonProperty(PropertyName = "amounts")]
        public Dictionary<string, decimal>? Amounts { get; set; }
    }
}
