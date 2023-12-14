// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;

using Newtonsoft.Json;

namespace BitPay.Models.Ledger
{
    public class LedgerEntry
    {
        [JsonProperty(PropertyName = "type")] public string? Type { get; set; }

        [JsonProperty(PropertyName = "amount")] public int? Amount { get; set; }
        
        [JsonProperty(PropertyName = "code")] public int? Code { get; set; }

        [JsonProperty(PropertyName = "timestamp")] public DateTime? Timestamp { get; set; }

        [JsonProperty(PropertyName = "currency")] public string? Currency { get; set; }

        [JsonProperty(PropertyName = "txType")] public string? TxType { get; set; }

        [JsonProperty(PropertyName = "scale")] public int? Scale { get; set; }

        [JsonProperty(PropertyName = "id")] public string? Id { get; set; }

        [JsonProperty(PropertyName = "supportRequest")] public string? SupportRequest { get; set; }

        [JsonProperty(PropertyName = "description")] public string? Description { get; set; }

        [JsonProperty(PropertyName = "invoiceId")] public string? InvoiceId { get; set; }

        [JsonProperty(PropertyName = "buyerFields")] public Buyer? Buyer { get; set; }

        [JsonProperty(PropertyName = "invoiceAmount")] public decimal? InvoiceAmount { get; set; }

        [JsonProperty(PropertyName = "invoiceCurrency")] public string? InvoiceCurrency { get; set; }

        [JsonProperty(PropertyName = "transactionCurrency")] public string? TransactionCurrency { get; set; }
    }
}