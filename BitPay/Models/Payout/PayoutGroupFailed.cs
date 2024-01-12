// Copyright (c) 2019 BitPay.
// All rights reserved.

using Newtonsoft.Json;

namespace BitPay.Models.Payout
{
    public class PayoutGroupFailed
    {
        [JsonProperty(PropertyName = "errMessage")]
        public string ErrMessage { get; set; }
    
        [JsonProperty(PropertyName = "payoutId")]
        public string? PayoutId { get; set; }
    
        [JsonProperty(PropertyName = "payee")]
        public string? Payee { get; set; }

        public PayoutGroupFailed(string errMessage, string? payoutId = null, string? payee = null)
        {
            ErrMessage = errMessage;
            PayoutId = payoutId;
            Payee = payee;
        }
    }
}