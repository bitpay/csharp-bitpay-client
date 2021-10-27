using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutCancellationException : PayoutException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-DELETE";
        private const string BitPayMessage = "Failed to create payout batch";

        public PayoutCancellationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutCancellationException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}