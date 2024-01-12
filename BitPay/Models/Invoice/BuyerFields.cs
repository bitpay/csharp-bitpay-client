// Copyright (c) 2019 BitPay.
// All rights reserved.

using Newtonsoft.Json;

namespace BitPay.Models.Invoice
{
    public class BuyerFields
    {
        [JsonProperty(PropertyName = "buyerName")]
        public string? BuyerName { get; set; }
        
        [JsonProperty(PropertyName = "buyerAddress1")]
        public string? BuyerAddress1 { get; set; }
        
        [JsonProperty(PropertyName = "buyerAddress2")]
        public string? BuyerAddress2 { get; set; }
        
        [JsonProperty(PropertyName = "buyerCity")]
        public string? BuyerCity { get; set; }
        
        [JsonProperty(PropertyName = "buyerState")]
        public string? BuyerState { get; set; }
        
        [JsonProperty(PropertyName = "buyerZip")]
        public string? BuyerZip { get; set; }
        
        [JsonProperty(PropertyName = "buyerCountry")]
        public string? BuyerCountry { get; set; }
        
        [JsonProperty(PropertyName = "buyerPhone")]
        public string? BuyerPhone { get; set; }
        
        [JsonProperty(PropertyName = "buyerNotify")]
        public bool? BuyerNotify { get; set; }
        
        [JsonProperty(PropertyName = "buyerEmail")]
        public string? BuyerEmail { get; set; }
    }
}