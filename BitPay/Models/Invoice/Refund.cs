// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;

using Newtonsoft.Json;

namespace BitPay.Models.Invoice
{
    public class Refund
    {
        // Request fields
        //

        [JsonProperty(PropertyName = "invoiceId")]
        public string? InvoiceId { get; set; }

        [JsonProperty(PropertyName = "preview")]
        public bool? Preview { get; set; }

        [JsonProperty(PropertyName = "immediate")]
        public bool? Immediate { get; set; }

        [JsonProperty(PropertyName = "buyerPaysRefundFee")]
        public bool? BuyerPaysRefundFee { get; set; }

        [JsonProperty(PropertyName = "guid")]
        public string? ResourceGuid { get; set; }

        [JsonProperty(PropertyName = "refundAddress")]
        public string? RefundAddress { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public decimal? Amount { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string? Currency { get; set; }
        
        [JsonProperty(PropertyName = "notificationURL")]
        public string? NotificationUrl { get; set; }

        // Response fields
        //

        [JsonProperty(PropertyName = "refundFee")]
        public decimal? RefundFee { get; set; }

        [JsonProperty(PropertyName = "lastRefundNotification")]
        public DateTime? LastRefundNotification { get; set; }

        [JsonProperty(PropertyName = "invoice")]
        public string? Invoice { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }

        [JsonProperty(PropertyName = "requestDate")]
        public DateTime? RequestDate { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string? Status { get; set; }

        [JsonProperty(PropertyName = "reference")]
        public string? Reference { get; set; }
        
        [JsonProperty(PropertyName = "transactionCurrency")]
        public string? TransactionCurrency { get; set; }
        
        [JsonProperty(PropertyName = "transactionAmount")]
        public decimal? TransactionAmount { get; set; }
        
        [JsonProperty(PropertyName = "transactionRefundFee")]
        public decimal? TransactionRefundFee { get; set; }
        
        [JsonProperty(PropertyName = "supportRequest")]
        public string? SupportRequest { get; set; }
        
        [JsonProperty(PropertyName = "txid")]
        public string? Txid { get; set; }
        
        [JsonProperty(PropertyName = "type")]
        public string? Type { get; set; }

        public Refund()
        {
        }

        public Refund(string? invoiceId, decimal? amount)
        {
            InvoiceId = invoiceId;
            Amount = amount;
        }

        public bool ShouldSerializeId()
        {
            return (Id != null);
        }

        public bool ShouldSerializeInvoiceId()
        {
            return (InvoiceId != null);
        }

        public bool ShouldSerializeRequestDate()
        {
            return (RequestDate != null);
        }

        public bool ShouldSerializeStatus()
        {
            return (Status != null);
        }

        public bool ShouldSerializeInvoice()
        {
            return (Invoice != null);
        }

        public bool ShouldSerializePreview()
        {
            return Preview.HasValue;
        }

        public bool ShouldSerializeImmediate()
        {
            return Immediate.HasValue;
        }

        public bool ShouldSerializeBuyerPaysRefundFee()
        {
            return BuyerPaysRefundFee.HasValue;
        }
    }
}
