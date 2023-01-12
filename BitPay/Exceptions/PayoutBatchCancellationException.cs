using System;

namespace BitPay.Exceptions
{
    public class PayoutBatchCancellationException : PayoutBatchException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-BATCH-CANCELLATION";
        private const string BitPayMessage = "Failed to cancel payout batch.";
        protected string ApiCode;

        public PayoutBatchCancellationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutBatchCancellationException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }

        public String GetApiCode()
        {
            return ApiCode;
        }
    }
}
