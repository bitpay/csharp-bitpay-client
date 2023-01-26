using System;

namespace BitPay.Exceptions
{
    public class RefundCancellationException : RefundException
    {
        private const string BitPayCode = "BITPAY-REFUND-CANCELLATION";
        private const string BitPayMessage = "Failed to cancel refund";

        public RefundCancellationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public RefundCancellationException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }
    }
}