using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutCreationException : BitPayException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-CREATE";
        private const string BitPayMessage = "Failed to create payout batch";

        public PayoutCreationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutCreationException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}