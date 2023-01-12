using System;

namespace BitPay.Exceptions
{
    public class RefundUpdateException : RefundException
    {
        private const string BitPayCode = "BITPAY-REFUND-UPDATE";
        private const string BitPayMessage = "Failed to update refund";

        public RefundUpdateException() : base(BitPayCode, BitPayMessage)
        {
        }

        public RefundUpdateException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}