// Copyright (c) 2019 BitPay.
// All rights reserved.

using Newtonsoft.Json;

namespace BitPay.Models.Wallet
{
    public class Qr
    {
        public string Type { get; set; }

        [JsonProperty(PropertyName = "collapsed")]
        public bool Collapsed { get; set; }

        public Qr(string type, bool collapsed)
        {
            Type = type;
            Collapsed = collapsed;
        }

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
