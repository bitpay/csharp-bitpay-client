using System;

namespace BitPay.Exceptions
{
    public class PayoutQueryException : PayoutException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-GET";
        private const string BitPayMessage = "Failed to retrieve payout.";

        public PayoutQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutQueryException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }
    }
}