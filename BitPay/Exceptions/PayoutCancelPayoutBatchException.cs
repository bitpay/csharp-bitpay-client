using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutCancelPayoutBatchException : PayoutBatchException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-CANCELLATION-BATCH";
        private const string BitPayMessage = "Failed to cancel payout batch";

        public PayoutCancelPayoutBatchException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutCancelPayoutBatchException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}