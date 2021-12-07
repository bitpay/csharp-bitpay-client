using System;

namespace BitPaySDK.Exceptions
{
    public class RefundQueryException : RefundException
    {
        private const string BitPayCode = "BITPAY-REFUND-GET";
        private const string BitPayMessage = "Failed to retrieve refund";
        protected string ApiCode;

        public RefundQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public RefundQueryException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }

        public String GetApiCode()
        {
            return ApiCode;
        }
    }
}