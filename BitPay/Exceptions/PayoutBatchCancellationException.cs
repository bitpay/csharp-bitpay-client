using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutBatchCancellationException : PayoutBatchException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-CANCELLATION-BATCH";
        private const string BitPayMessage = "Failed to cancel payout batch";

        public PayoutBatchCancellationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutBatchCancellationException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}
