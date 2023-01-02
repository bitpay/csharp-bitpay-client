using System;

namespace BitPay.Exceptions
{
    public class PayoutUpdateException : PayoutException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-UPDATE";
        private const string BitPayMessage = "Failed to update payout.";
        protected string ApiCode;

        public PayoutUpdateException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutUpdateException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }

        public String GetApiCode()
        {
            return ApiCode;
        }
    }
}
