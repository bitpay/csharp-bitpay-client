// Copyright (c) 2019 BitPay.
// All rights reserved.

using Newtonsoft.Json;

namespace BitPay.Models.Settlement
{
    public class WithHoldings
    {
        [JsonProperty(PropertyName = "amount")]
        public decimal? Amount { get; set; }
        
        [JsonProperty(PropertyName = "code")]
        public string? Code { get; set; }
        
        [JsonProperty(PropertyName = "description")]
        public string? Description { get; set; }
        
        [JsonProperty(PropertyName = "notes")]
        public string? Notes { get; set; }
        
        [JsonProperty(PropertyName = "label")]
        public string? Label { get; set; }
        
        [JsonProperty(PropertyName = "bankCountry")]
        public string? BankCountry { get; set; }
    }
}