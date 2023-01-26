using Newtonsoft.Json;

namespace BitPay.Models.Payout
{
    public class PayoutInstructionBtcSummary
    {
        public PayoutInstructionBtcSummary(decimal paid, decimal unpaid)
        {
            Paid = paid;
            Unpaid = unpaid;
        }

        [JsonProperty(PropertyName = "paid")]
        [JsonConverter(typeof(Converters.BtcValueConverter))]
        public decimal Paid { get; set; }

        [JsonProperty(PropertyName = "unpaid")]
        [JsonConverter(typeof(Converters.BtcValueConverter))]
        public decimal Unpaid { get; set; }
    }
}