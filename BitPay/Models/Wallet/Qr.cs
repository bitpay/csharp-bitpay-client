// Copyright (c) 2019 BitPay.
// All rights reserved.

using Newtonsoft.Json;

namespace BitPay.Models.Wallet
{
    public class Qr
    {
        [JsonProperty(PropertyName = "type")]
        public string? Type { get; set; }

        [JsonProperty(PropertyName = "collapsed")]
        public bool? Collapsed { get; set; }

        public bool ShouldSerializeType()
        {
            return !string.IsNullOrEmpty(Type); 
        }

        public bool ShouldSerializeCollapsed()
        {
            return false;
        }
    }
}
