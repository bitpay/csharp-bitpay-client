// Copyright (c) 2019 BitPay.
// All rights reserved.

using System.Collections.Generic;

using Newtonsoft.Json;

namespace BitPay.Models.Wallet
{
    
    public class Wallet
    {
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }
        
        [JsonProperty(PropertyName = "displayName")]
        public string? DisplayName { get; set; }
        
        [JsonProperty(PropertyName = "avatar")]
        public string? Avatar { get; set; }
        
        [JsonProperty(PropertyName = "image")]
        public string? Image { get; set; }

        [JsonProperty(PropertyName = "payPro")]
        public bool? PayPro { get; set; }

        [JsonProperty(PropertyName = "currencies")]
        public List<Currencies>? Currencies { get; set; }

        public Wallet(string key)
        {
            Key = key;
        }

        public bool ShouldSerializeKey()
        {
            return !string.IsNullOrEmpty(Key);
        }

        public bool ShouldSerializeDisplayName()
        {
            return !string.IsNullOrEmpty(DisplayName);
        }

        public bool ShouldSerializeAvatar()
        {
            return !string.IsNullOrEmpty(Avatar);
        }

        public bool ShouldSerializePayPro()
        {
            return false;
        }

        public bool ShouldSerializeCurrencies()
        {
            return false;
        }
    }
}

