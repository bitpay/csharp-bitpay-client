using System;

namespace BitPay.Exceptions
{

    public class PayoutBatchNotificationException : PayoutBatchException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-BATCH-NOTIFICATION";
        private const string BitPayMessage = "Failed to send payout batch notification.";

        public PayoutBatchNotificationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutBatchNotificationException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }
    }
}
