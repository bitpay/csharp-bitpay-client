using System;

namespace BitPaySDK.Exceptions
{
    public class RefundCreationException : BitPayException
    {
        private const string BitPayCode = "BITPAY-REFUND-CREATE";
        private const string BitPayMessage = "Failed to create refund";

        public RefundCreationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public RefundCreationException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}