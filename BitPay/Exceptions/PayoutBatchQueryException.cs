using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutBatchQueryException : PayoutBatchException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-BATCH-GET";
        private const string BitPayMessage = "Failed to retrieve payout batch";

        public PayoutBatchQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutBatchQueryException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}
