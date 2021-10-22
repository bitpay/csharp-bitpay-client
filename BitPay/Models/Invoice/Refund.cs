using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BitPaySDK.Models.Invoice
{
    public class Refund
    {
        public Refund() {
        }

        // Request fields
        //

        [JsonProperty(PropertyName = "invoiceId")]
        public string InvoiceId { get; set; }

        [JsonProperty(PropertyName = "preview")]
        public bool Preview { get; set; }

        [JsonProperty(PropertyName = "immediate")]
        public bool Immediate { get; set; }

        [JsonProperty(PropertyName = "buyerPaysRefundFee")]
        public bool BuyerPaysRefundFee { get; set; }

        [JsonProperty(PropertyName = "guid")]
        public string Guid { get; set; }

        [JsonProperty(PropertyName = "refundEmail")]
        public string RefundEmail { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public double Amount { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        // Response fields
        //

        [JsonProperty(PropertyName = "refundFee")]
        public double RefundFee { get; set; }

        [JsonProperty(PropertyName = "lastRefundNotification")]
        public DateTime LastRefundNotification { get; set; }

        [JsonProperty(PropertyName = "invoice")]
        public string Invoice { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "requestDate")]
        public string RequestDate { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "params")]
        public RefundParams PaymentUrls { get; set; }

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

        public bool ShouldSerializePaymentUrls()
        {
            return (PaymentUrls != null);
        }

        public bool ShouldSerializePreview()
        {
            return false;
        }

        public bool ShouldSerializeImmediate()
        {
            return false;
        }

        public bool ShouldSerializeBuyerPaysRefundFee()
        {
            return false;
        }
    }
}
