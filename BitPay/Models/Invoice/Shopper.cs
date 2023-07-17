// Copyright (c) 2019 BitPay.
// All rights reserved.

using Newtonsoft.Json;

namespace BitPay.Models.Invoice
{
    public class Shopper
    {
        [JsonProperty(PropertyName = "user")]
        public string? User { get; set; }
    }
}
