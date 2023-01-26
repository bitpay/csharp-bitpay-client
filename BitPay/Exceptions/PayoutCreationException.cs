using System;

namespace BitPay.Exceptions
{
    public class PayoutCreationException : PayoutException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-CREATE";
        private const string BitPayMessage = "Failed to create payout.";

        public PayoutCreationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutCreationException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }
    }
}