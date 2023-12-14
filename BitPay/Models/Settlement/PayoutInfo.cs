// Copyright (c) 2019 BitPay.
// All rights reserved.

using Newtonsoft.Json;

namespace BitPay.Models.Settlement
{
    public class PayoutInfo
    {
        [JsonProperty(PropertyName = "name")]
        public string? Name { get; set; }
        
        [JsonProperty(PropertyName = "account")]
        public string? Account { get; set; }
        
        [JsonProperty(PropertyName = "routung")]
        public string? Routing { get; set; }
        
        [JsonProperty(PropertyName = "merchantEin")]
        public string? MerchantEin { get; set; }
        
        [JsonProperty(PropertyName = "label")]
        public string? Label { get; set; }
        
        [JsonProperty(PropertyName = "bankCountry")]
        public string? BankCountry { get; set; }
        
        [JsonProperty(PropertyName = "bank")]
        public string? Bank { get; set; }
        
        [JsonProperty(PropertyName = "swift")]
        public string? Swift { get; set; }
        
        [JsonProperty(PropertyName = "address")]
        public string? Address { get; set; }
        
        [JsonProperty(PropertyName = "city")]
        public string? City { get; set; }
        
        [JsonProperty(PropertyName = "postal")]
        public string? Postal { get; set; }

        [JsonProperty(PropertyName = "sort")]
        public string? Sort { get; set; }
        
        [JsonProperty(PropertyName = "wire")]
        public bool? Wire { get; set; }
        
        [JsonProperty(PropertyName = "bankName")]
        public string? BankName { get; set; }
        
        [JsonProperty(PropertyName = "bankAddress")]
        public string? BankAddress { get; set; }
        
        [JsonProperty(PropertyName = "bankAddress2")]
        public string? BankAddress2 { get; set; }
        
        [JsonProperty(PropertyName = "iban")]
        public string? Iban { get; set; }
        
        [JsonProperty(PropertyName = "additionalInformation")]
        public string? AdditionalInformation { get; set; }
        
        [JsonProperty(PropertyName = "accountHolderName")]
        public string? AccountHolderName { get; set; }
        
        [JsonProperty(PropertyName = "accountHolderAddress")]
        public string? AccountHolderAddress { get; set; }
        
        [JsonProperty(PropertyName = "accountHolderAddress2")]
        public string? AccountHolderAddress2 { get; set; }
        
        [JsonProperty(PropertyName = "accountHolderPostalCode")]
        public string? AccountHolderPostalCode { get; set; }
        
        [JsonProperty(PropertyName = "accountHolderCity")]
        public string? AccountHolderCity { get; set; }
        
        [JsonProperty(PropertyName = "accountHolderCountry")]
        public string? AccountHolderCountry { get; set; }
    }
}
