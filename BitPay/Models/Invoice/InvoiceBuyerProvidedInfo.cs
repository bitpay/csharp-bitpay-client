// Copyright (c) 2019 BitPay.
// All rights reserved.

using Newtonsoft.Json;

namespace BitPay.Models.Invoice
{
    public class InvoiceBuyerProvidedInfo
    {
        [JsonProperty(PropertyName = "name")]
        public string? Name { get; set; }
        
        [JsonProperty(PropertyName = "sms")]
        public string? Sms { get; set; }

        [JsonProperty(PropertyName = "phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "emailAddress")]
        public string? EmailAddress { get; set; }

        [JsonProperty(PropertyName = "selectedWallet")]
        public string? SelectedWallet { get; set; }

        [JsonProperty(PropertyName = "selectedTransactionCurrency")]
        public string? SelectedTransactionCurrency { get; set; }
        
        [JsonProperty(PropertyName = "smsVerified")]
        public bool? SmsVerified { get; set; }
        
        public bool ShouldSerializeEmailAddress()
        {
            return !string.IsNullOrEmpty(EmailAddress);
        }
        
        public bool ShouldSerializeName()
        {
            return !string.IsNullOrEmpty(Name);
        }
        
        public bool ShouldSerializePhoneNumber()
        {
            return !string.IsNullOrEmpty(PhoneNumber);
        }
        
        public bool ShouldSerializeSelectedTransactionCurrency()
        {
            return !string.IsNullOrEmpty(SelectedTransactionCurrency);
        }
        
        public bool ShouldSerializeSelectedWallet()
        {
            return !string.IsNullOrEmpty(SelectedWallet);
        }
        
        public bool ShouldSerializeSms()
        {
            return !string.IsNullOrEmpty(Sms);
        }
    }
}
