using Newtonsoft.Json;

namespace BitPay.Models.Ledger
{
    public class Ledger
    {
        [JsonProperty(PropertyName = "currency")] public string Currency { get; set; }
        
        [JsonProperty(PropertyName = "balance")] public decimal Balance { get; set; }
    }
}