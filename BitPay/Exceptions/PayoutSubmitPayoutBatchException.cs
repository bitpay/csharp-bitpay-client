using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutSubmitPayoutBatchException : PayoutBatchException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-SUBMIT-BATCH";
        private const string BitPayMessage = "Failed to create payout batch";

        public PayoutSubmitPayoutBatchException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutSubmitPayoutBatchException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}