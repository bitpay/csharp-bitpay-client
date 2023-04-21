// Copyright (c) 2019 BitPay.
// All rights reserved.

using Newtonsoft.Json;

namespace BitPay.Models.Payout
{
    public class PayoutRecipient
    {
        /// <summary>
        ///     Constructor, create an empty PayoutRecipient object.
        /// </summary>
        public PayoutRecipient()
        {
        }
        
        /**
         * Constructor, create a minimal Recipient object.
         *
         * @param email           string Recipient email address to which the invite shall be sent.
         * @param label           string Recipient nickname assigned by the merchant (Optional).
         */
        public PayoutRecipient(string email, string label) {
            Email = email;
            Label = label;
        }

        // Required fields
        //

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        // Optional fields
        //

        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        [JsonProperty(PropertyName = "notificationURL")]
        public string NotificationURL { get; set; }

        // Response fields
        //

        public string Status { get; set; }

        public string Id { get; set; }

        public string ShopperId { get; set; }

        [JsonProperty(PropertyName = "token")] public string Token { get; set; }

        public bool ShouldSerializeEmail()
        {
            return !string.IsNullOrEmpty(Email);
        }

        public bool ShouldSerializeLabel()
        {
            return !string.IsNullOrEmpty(Label);
        }

        public bool ShouldSerializeNotificationURL()
        {
            return !string.IsNullOrEmpty(NotificationURL);
        }

        public bool ShouldSerializeStatus()
        {
            return false;
        }

        public bool ShouldSerializeId()
        {
            return false;
        }

        public bool ShouldSerializeShopperId()
        {
            return false;
        }

    }
}
