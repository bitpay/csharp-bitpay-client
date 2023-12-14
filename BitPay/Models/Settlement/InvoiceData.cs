// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace BitPay.Models.Settlement
{
    public class InvoiceData
    {
        [JsonProperty(PropertyName = "amount")]
        public decimal? Amount { get; set; }
        
        [JsonProperty(PropertyName = "orderId")]
        public string? OrderId { get; set; }
        
        [JsonProperty(PropertyName = "date")]
        public DateTime? Date { get; set; }
        
        [JsonProperty(PropertyName = "price")]
        public decimal? Price { get; set; }
        
        [JsonProperty(PropertyName = "currency")]
        public string? Currency { get; set; }
        
        [JsonProperty(PropertyName = "transactionCurrency")]
        public string? TransactionCurrency { get; set; }
        
        [JsonProperty(PropertyName = "overPaidAmount")]
        public decimal? OverPaidAmount { get; set; }
        
        [JsonProperty(PropertyName = "payoutPercentage")]
        public Dictionary<string, decimal>? PayoutPercentage { get; set; }

        [JsonProperty(PropertyName = "refundInfo")]
        public RefundInfo? RefundInfo { get; set; }
    }
}