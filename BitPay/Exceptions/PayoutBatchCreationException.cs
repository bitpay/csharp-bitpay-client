using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutBatchCreationException : PayoutBatchException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-BATCH-CREATE";
        private const string BitPayMessage = "Failed to create payout batch.";

        public PayoutBatchCreationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutBatchCreationException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}
