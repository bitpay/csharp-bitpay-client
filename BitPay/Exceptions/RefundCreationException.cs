using System;

namespace BitPay.Exceptions
{
    public class RefundCreationException : RefundException
    {
        private const string BitPayCode = "BITPAY-REFUND-CREATE";
        private const string BitPayMessage = "Failed to create refund";

        public RefundCreationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public RefundCreationException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }
    }
}