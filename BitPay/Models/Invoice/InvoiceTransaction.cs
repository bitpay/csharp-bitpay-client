// Copyright (c) 2019 BitPay.
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
        public decimal? Amount { get; set; }

        [JsonProperty(PropertyName = "confirmations")]
        public int? Confirmations { get; set; }

        [JsonProperty(PropertyName = "receivedTime")]
        public string? ReceivedTime { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string? Type { get; set; }

        [JsonProperty(PropertyName = "txid")]
        public string? Txid { get; set; }
        
        [JsonProperty(PropertyName = "refundAmount")]
        public decimal? RefundAmount { get; set; }

        [JsonProperty(PropertyName = "time")]
        public string? Time { get; set; }

        [JsonProperty(PropertyName = "exRates")]
        public dynamic? ExchangeRates {
            get => _exchangeRates;
            set => _exchangeRates = JsonConvert.DeserializeObject(value?.ToString(Formatting.None));
        }

        [JsonProperty(PropertyName = "outputIndex")]
        public int? OutputIndex { get; set; }

        public bool ShouldSerializeAmount()
        {
            return Amount.HasValue;
        }
        
        public bool ShouldSerializeConfirmations()
        {
            return Confirmations.HasValue;
        }
        
        public bool ShouldSerializeExchangeRates()
        {
            return !string.IsNullOrEmpty(ExchangeRates);
        }
        
        public bool ShouldSerializeOutputIndex()
        {
            return OutputIndex.HasValue;
        }
        
        public bool ShouldSerializeReceivedTime()
        {
            return !string.IsNullOrEmpty(ReceivedTime);
        }
        
        public bool ShouldSerializeRefundAmount()
        {
            return RefundAmount.HasValue;
        }
        
        public bool ShouldSerializeTime()
        {
            return !string.IsNullOrEmpty(Time);
        }
        
        public bool ShouldSerializeTxid()
        {
            return !string.IsNullOrEmpty(Txid);
        }
        
        public bool ShouldSerializeType()
        {
            return !string.IsNullOrEmpty(Type);
        }
    }
}