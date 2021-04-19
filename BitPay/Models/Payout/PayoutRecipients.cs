using System.Collections.Generic;
using Newtonsoft.Json;

namespace BitPaySDK.Models.Payout
{
    public class PayoutRecipients
    {
        /**
         * Constructor, create an recipient-full request PayoutBatch object.
         *
         * @param recipients array array of JSON objects, with containing the following parameters.
         */
        public PayoutRecipients(List<PayoutRecipient> recipients) {
            Recipients = recipients;
        }

        // API fields
        //

        [JsonProperty(PropertyName = "guid")] public string Guid { get; set; }

        [JsonProperty(PropertyName = "token")] public string Token { get; set; }

        // Required fields
        //

        [JsonProperty(PropertyName = "recipients")]
        public List<PayoutRecipient> Recipients { get; set; }

        public bool ShouldSerializeRecipients()
        {
            return true;
        }
    }
}