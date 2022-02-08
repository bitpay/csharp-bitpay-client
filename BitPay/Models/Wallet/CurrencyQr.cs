using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BitPaySDK.Models.Wallet
{
    public class CurrencyQr
    {
        public string Type { get; set; }

        [JsonProperty(PropertyName = "collapsed")]
        public bool Collapsed { get; set; }

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
