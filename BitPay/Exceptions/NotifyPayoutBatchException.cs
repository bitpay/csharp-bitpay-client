using System;

namespace BitPaySDK.Exceptions
{

    public class NotifyPayoutBatchException : PayoutException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-BATCH-NOTIFICATION";
        private const string BitPayMessage = "Failed to send payout batch notification.";

        public NotifyPayoutBatchException() : base(BitPayCode, BitPayMessage)
        {
        }

        public NotifyPayoutBatchException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}
