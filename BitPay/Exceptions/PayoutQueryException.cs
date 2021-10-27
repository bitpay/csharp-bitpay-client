using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutQueryException : PayoutException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-GET";
        private const string BitPayMessage = "Failed to create payout batch";

        public PayoutQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutQueryException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}