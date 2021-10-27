using System;

namespace BitPaySDK.Exceptions
{

    public class NotifyPayoutRecipientException : PayoutException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-RECIPIENT-NOTIFICATION";
        private const string BitPayMessage = "Failed to send payout recipient notification.";

        public NotifyPayoutRecipientException() : base(BitPayCode, BitPayMessage)
        {
        }

        public NotifyPayoutRecipientException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}
