using System;

namespace BitPaySDK.Exceptions
{
    public class RefundUpdateException : RefundException
    {
        private const string BitPayCode = "BITPAY-REFUND-GET";
        private const string BitPayMessage = "Failed to update refund";

        public RefundUpdateException() : base(BitPayCode, BitPayMessage)
        {
        }

        public RefundUpdateException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}