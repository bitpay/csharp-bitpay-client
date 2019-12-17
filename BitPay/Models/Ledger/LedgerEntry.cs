using System.Collections.Generic;
using Newtonsoft.Json;

namespace BitPaySDK.Models.Ledger
{
    public class LedgerEntry
    {
        [JsonProperty(PropertyName = "type")] public string Type { get; set; }

        [JsonProperty(PropertyName = "amount")] public string Amount { get; set; }
        
        [JsonProperty(PropertyName = "code")] public string Code { get; set; }

        [JsonProperty(PropertyName = "timestamp")] public string Timestamp { get; set; }

        [JsonProperty(PropertyName = "currency")] public string Currency { get; set; }

        [JsonProperty(PropertyName = "txType")] public string TxType { get; set; }

        [JsonProperty(PropertyName = "scale")] public string Scale { get; set; }

        [JsonProperty(PropertyName = "id")] public string Id { get; set; }

        [JsonProperty(PropertyName = "supportRequest")] public string supportRequest { get; set; }

        [JsonProperty(PropertyName = "description")] public string Description { get; set; }

        [JsonProperty(PropertyName = "invoiceId")] public string InvoiceId { get; set; }

        [JsonProperty(PropertyName = "buyerFields")] public Buyer Buyer { get; set; }

        [JsonProperty(PropertyName = "invoiceAmount")] public double InvoiceAmount { get; set; }

        [JsonProperty(PropertyName = "invoiceCurrency")] public string InvoiceCurrency { get; set; }

        [JsonProperty(PropertyName = "transactionCurrency")] public string TransactionCurrency { get; set; }
    }
}