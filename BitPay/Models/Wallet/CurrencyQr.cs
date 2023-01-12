using Newtonsoft.Json;

namespace BitPay.Models.Wallet
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
