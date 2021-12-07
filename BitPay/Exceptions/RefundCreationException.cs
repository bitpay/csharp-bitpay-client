using System;

namespace BitPaySDK.Exceptions
{
    public class RefundCreationException : RefundException
    {
        private const string BitPayCode = "BITPAY-REFUND-CREATE";
        private const string BitPayMessage = "Failed to create refund";
        protected string ApiCode;

        public RefundCreationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public RefundCreationException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }

        public String GetApiCode()
        {
            return ApiCode;
        }
    }
}