using System;

namespace BitPaySDK.Exceptions
{

    public class NotifyPayoutException : PayoutException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-NOTIFICATION";
        private const string BitPayMessage = "Failed to send payout notification.";

        public NotifyPayoutException() : base(BitPayCode, BitPayMessage)
        {
        }

        public NotifyPayoutException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}
