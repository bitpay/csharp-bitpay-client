using Newtonsoft.Json;
using System.Collections.Generic;

namespace BitPaySDK.Models.Invoice
{
    public class RefundInfo
    {
        [JsonProperty(PropertyName = "supportRequest")]
        public string SupportRequest { get; set; }
        
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
        
        [JsonProperty(PropertyName = "amounts")]
        public Dictionary<string, double> Amounts { get; set; }
    }
}
