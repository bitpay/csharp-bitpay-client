using System;

namespace BitPay.Exceptions
{
    public class PayoutCancellationException : PayoutException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-CANCELLATION";
        private const string BitPayMessage = "Failed to cancel payout.";

        public PayoutCancellationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutCancellationException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }
    }
}
