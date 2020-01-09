using Newtonsoft.Json;

namespace BitPaySDK.Models.Invoice
{
    public class RefundParams
    {
        public RefundParams() {
        }

        [JsonProperty(PropertyName = "requesterType")]
        public string setRequesterType { get; set; }

        [JsonProperty(PropertyName = "requesterEmail")]
        public string setRequesterEmail { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public double setAmount { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string setCurrency { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string setEmail { get; set; }

        [JsonProperty(PropertyName = "purchaserNotifyEmail")]
        public string setPurchaserNotifyEmail { get; set; }

        [JsonProperty(PropertyName = "refundAddress")]
        public string setRefundAddress { get; set; }

        [JsonProperty(PropertyName = "supportRequestEid")]
        public string setSupportRequestEid { get; set; }
    }
}
