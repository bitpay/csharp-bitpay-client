using Newtonsoft.Json;

namespace BitPaySDK.Models.Invoice
{
    public class Refund
    {
        public Refund() {
        }

        // Request fields
        //

        [JsonProperty(PropertyName = "guid")]
        public string Guid { get; set; }

        [JsonProperty(PropertyName = "refundEmail")]
        public string RefundEmail { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public double Amount { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        // Response fields
        //

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "requestDate")]
        public string RequestDate { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "params")]
        public RefundParams PaymentUrls { get; set; }

        public bool ShouldSerializeId()
        {
            return (Id != null);
        }

        public bool ShouldSerializeRequestDate()
        {
            return (RequestDate != null);
        }

        public bool ShouldSerializeStatus()
        {
            return (Status != null);
        }

        public bool ShouldSerializePaymentUrls()
        {
            return (PaymentUrls != null);
        }
    }
}
