using System;

namespace BitPaySDK.Exceptions
{

    public class PayoutNotificationException : PayoutException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-NOTIFICATION";
        private const string BitPayMessage = "Failed to send payout notification.";

        public PayoutNotificationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutNotificationException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}
