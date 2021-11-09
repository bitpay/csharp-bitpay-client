using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutCancellationException : PayoutException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-CANCELLATION";
        private const string BitPayMessage = "Failed to cancel payout.";

        public PayoutCancellationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutCancellationException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}
