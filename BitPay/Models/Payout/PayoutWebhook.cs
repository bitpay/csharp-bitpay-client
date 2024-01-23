// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Collections.Generic;

using BitPay.Models.Invoice;

using Newtonsoft.Json;

namespace BitPay.Models.Payout
{
    public class PayoutWebhook
    {
        [JsonProperty(PropertyName = "currency")]
        public string? Currency { get; set; }
        
        [JsonProperty(PropertyName = "effectiveDate")]
        public DateTime? EffectiveDate { get; set; }
        
        [JsonProperty(PropertyName = "email")]
        public string? Email { get; set; }
        
        [JsonProperty(PropertyName = "exchangeRates")]
        public Dictionary<string, Dictionary<string, decimal>>? ExchangeRates { get; set; }
        
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        
        [JsonProperty(PropertyName = "label")]
        public string? Label { get; set; }
        
        [JsonProperty(PropertyName = "ledgerCurrency")]
        public string? LedgerCurrency { get; set; }
        
        [JsonProperty(PropertyName = "notificationEmail")]
        public string? NotificationEmail { get; set; }
        
        [JsonProperty(PropertyName = "notificationURL")]
        public string? NotificationUrl { get; set; }
        
        [JsonProperty(PropertyName = "price")]
        public decimal? Price { get; set; }
        
        [JsonProperty(PropertyName = "recipientId")]
        public string? RecipientId { get; set; }
        
        [JsonProperty(PropertyName = "reference")]
        public string? Reference { get; set; }
        
        [JsonProperty(PropertyName = "requestDate")]
        public DateTime? RequestDate { get; set; }
        
        [JsonProperty(PropertyName = "shopperId")]
        public string? ShopperId { get; set; }
        
        [JsonProperty(PropertyName = "status")]
        public string? Status { get; set; }
        
        [JsonProperty(PropertyName = "transactions")]
        public List<InvoiceTransaction>? Transactions { get; set; }

        [JsonProperty(PropertyName = "accountId")]
        public string? AccountId { get; set; }

        [JsonProperty(PropertyName = "date")]
        public DateTime? Date { get; set; }

        [JsonProperty(PropertyName = "groupId")]
        public string? GroupId { get; set; }
    }
}