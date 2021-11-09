using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutUpdateException : PayoutException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-UPDATE";
        private const string BitPayMessage = "Failed to update payout.";

        public PayoutUpdateException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutUpdateException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}
