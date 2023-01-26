using Newtonsoft.Json;

namespace BitPay.Models.Ledger
{
    public class Ledger
    {
        [JsonProperty(PropertyName = "currency")] public string Currency { get; set; }
        
        [JsonProperty(PropertyName = "balance")] public double Balance { get; set; }
    }
}