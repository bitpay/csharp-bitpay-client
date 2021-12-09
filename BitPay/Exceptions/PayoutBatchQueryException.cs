using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutBatchQueryException : PayoutBatchException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-BATCH-GET";
        private const string BitPayMessage = "Failed to retrieve payout batch";
        protected string ApiCode;

        public PayoutBatchQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutBatchQueryException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }

        public String GetApiCode()
        {
            return ApiCode;
        }
    }
}
