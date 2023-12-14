// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;

using Newtonsoft.Json;

namespace BitPay.Models.Payout
{
    public class PayoutInstructionTransaction
    {
        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; }
        
        [JsonProperty(PropertyName = "txid")]
        public string? Txid { get; set; }

        [JsonProperty(PropertyName = "date")]
        public DateTime? Date { get; set; }
        
        [JsonProperty(PropertyName = "confirmations")]
        public string? Confirmations { get; set; }

        public PayoutInstructionTransaction(decimal amount)
        {
            Amount = amount;
        }
    }
}