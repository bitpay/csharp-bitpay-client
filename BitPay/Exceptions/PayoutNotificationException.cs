using System;

namespace BitPay.Exceptions
{

    public class PayoutNotificationException : PayoutException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-NOTIFICATION";
        private const string BitPayMessage = "Failed to send payout notification.";

        public PayoutNotificationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutNotificationException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }
    }
}
