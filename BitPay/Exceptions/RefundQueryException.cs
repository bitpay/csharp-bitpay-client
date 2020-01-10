using System;

namespace BitPaySDK.Exceptions
{
    public class RefundQueryException : RefundException
    {
        private const string BitPayCode = "BITPAY-REFUND-GET";
        private const string BitPayMessage = "Failed to retrieve refund";

        public RefundQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public RefundQueryException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}