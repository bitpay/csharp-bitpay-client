// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;

using Newtonsoft.Json;

namespace BitPay.Models.Invoice
{
    public class RefundWebhook
    {
        [JsonProperty(PropertyName = "amount")]
        public decimal? Amount { get; set; }
        
        [JsonProperty(PropertyName = "buyerPaysRefundFee")]
        public bool? BuyerPaysRefundFee { get; set; }
        
        [JsonProperty(PropertyName = "currency")]
        public string? Currency { get; set; }
        
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        
        [JsonProperty(PropertyName = "immediate")]
        public bool? Immediate { get; set; }
        
        [JsonProperty(PropertyName = "invoice")]
        public string? Invoice { get; set; }
        
        [JsonProperty(PropertyName = "lastRefundNotification")]
        public DateTime? LastRefundNotification { get; set; }
        
        [JsonProperty(PropertyName = "refundFee")]
        public decimal? RefundFee { get; set; }
        
        [JsonProperty(PropertyName = "requestDate")]
        public DateTime? RequestDate { get; set; }
        
        [JsonProperty(PropertyName = "status")]
        public string? Status { get; set; }
        
        [JsonProperty(PropertyName = "supportRequest")]
        public string? SupportRequest { get; set; }
    }
}