// Copyright (c) 2019 BitPay.
// All rights reserved.

using System.Collections.Generic;

using Newtonsoft.Json;

namespace BitPay.Models.Wallet
{
    
    public class Wallet
    {
        /// <summary>
        ///     Creates an uninitialized wallet request object.
        /// </summary>
        public Wallet()
        {
        }
        public string Key { get; set; }
        public string DisplayName { get; set; }
        public string Avatar { get; set; }
        public string Image { get; set; }

        [JsonProperty(PropertyName = "payPro")]
        public bool PayPro { get; set; }
        public List<Currencies> Currencies { get; set; }

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

