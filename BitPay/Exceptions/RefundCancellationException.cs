using System;

namespace BitPaySDK.Exceptions
{
    public class RefundCancellationException : BitPayException
    {
        private const string BitPayCode = "BITPAY-REFUND-CANCELLATION";
        private const string BitPayMessage = "Failed to cancel refund";

        public RefundCancellationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public RefundCancellationException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}