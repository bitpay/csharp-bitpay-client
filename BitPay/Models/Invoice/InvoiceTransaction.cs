﻿// Copyright (c) 2019 BitPay.
// All rights reserved.

using Newtonsoft.Json;

namespace BitPay.Models.Invoice
{
    /// <summary>
    ///     Provides information about a single invoice transaction.
    /// </summary>
    public class InvoiceTransaction
    {
        private dynamic? _exchangeRates;
        
        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; }

        [JsonProperty(PropertyName = "confirmations")]
        public string Confirmations { get; set; }

        [JsonProperty(PropertyName = "receivedTime")]
        public string ReceivedTime { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string? Type { get; set; }

        [JsonProperty(PropertyName = "txid")]
        public string Txid { get; set; }
        
        [JsonProperty(PropertyName = "refundAmount")]
        public decimal RefundAmount { get; set; }

        [JsonProperty(PropertyName = "time")]
        public string? Time { get; set; }

        [JsonProperty(PropertyName = "exRates")]
        public dynamic? ExchangeRates {
            get => _exchangeRates;
            set => _exchangeRates = JsonConvert.DeserializeObject(value?.ToString(Formatting.None));
        }

        [JsonProperty(PropertyName = "outputIndex")]
        public int OutputIndex { get; set; }

        public InvoiceTransaction(
            decimal amount,
            string confirmations,
            string receivedTime,
            string txid, 
            decimal refundAmount,
            int outputIndex
        )
        {
            Amount = amount;
            Confirmations = confirmations;
            ReceivedTime = receivedTime;
            Txid = txid;
            RefundAmount = refundAmount;
            OutputIndex = outputIndex;
        }
    }
}