// Copyright (c) 2019 BitPay.
// All rights reserved.

using Newtonsoft.Json;

namespace BitPay.Models.Payout
{
    public class RecipientWebhook
    {
        [JsonProperty(PropertyName = "email")]
        public string? Email { get; set; }
        
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        
        [JsonProperty(PropertyName = "label")]
        public string? Label { get; set; }
        
        [JsonProperty(PropertyName = "shopperId")]
        public string? ShopperId { get; set; }
        
        [JsonProperty(PropertyName = "status")]
        public string? Status { get; set; }
        

    }
}