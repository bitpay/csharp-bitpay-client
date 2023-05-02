// Copyright (c) 2019 BitPay.
// All rights reserved.

using System.Collections.Generic;

using Newtonsoft.Json;

namespace BitPay.Models.Invoice
{
    public class InvoiceEventToken
    {
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
        
        [JsonProperty(PropertyName = "events")]
        public List<string> Events { get; set; }
        
        [JsonProperty(PropertyName = "actions")]
        public List<string> Actions { get; set; }
    }
}