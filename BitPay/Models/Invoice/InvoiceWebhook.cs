// Copyright (c) 2019 BitPay.
// All rights reserved.

using System.Collections.Generic;
using System.Numerics;

using Newtonsoft.Json;

namespace BitPay.Models.Invoice
{
    public class InvoiceWebhook
    {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string? Url { get; set; }

        [JsonProperty(PropertyName = "posData")]
        public string? PosData { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string? Status { get; set; }

        [JsonProperty(PropertyName = "price")]
        public double? Price { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string? Currency { get; set; }

        [JsonProperty(PropertyName = "invoiceTime")]
        public string? InvoiceTime { get; set; }

        [JsonProperty(PropertyName = "currencyTime")]
        public string? CurrencyTime { get; set; }

        [JsonProperty(PropertyName = "exceptionStatus")]
        public string? ExceptionStatus { get; set; }

        [JsonProperty(PropertyName = "buyerFields")]
        public BuyerFields? BuyerFields { get; set; }

        [JsonProperty(PropertyName = "paymentSubtotals")]
        public Dictionary<string, BigInteger>? PaymentSubtotals { get; set; }

        [JsonProperty(PropertyName = "paymentTotals")]
        public Dictionary<string, BigInteger>? PaymentTotals { get; set; }

        [JsonProperty(PropertyName = "exchangeRates")]
        public Dictionary<string, Dictionary<string, decimal>>? ExchangeRates { get; set; }

        [JsonProperty(PropertyName = "amountPaid")]
        public double? AmountPaid { get; set; }

        [JsonProperty(PropertyName = "orderId")]
        public string? OrderId { get; set; }

        [JsonProperty(PropertyName = "transactionCurrency")]
        public string? TransactionCurrency { get; set; }

        [JsonProperty(PropertyName = "inInvoiceId")]
        public string? InInvoiceId { get; set; }

        [JsonProperty(PropertyName = "inPaymentRequest")]
        public string? InPaymentRequest { get; set; }
    }
}