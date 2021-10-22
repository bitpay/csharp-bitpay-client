using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BitPaySDK.Models.Invoice
{
    public class UniversalCodes
    {
        [JsonProperty(PropertyName = "paymentString")] public string PaymentString { get; set; }

        [JsonProperty(PropertyName = "verificationLink")] public string VerificationLink { get; set; }

        public bool ShouldSerializePaymentString()
        {
            return !string.IsNullOrEmpty(PaymentString);
        }

        public bool ShouldSerializeVerificationLink()
        {
            return !string.IsNullOrEmpty(VerificationLink);
        }
    }
}
