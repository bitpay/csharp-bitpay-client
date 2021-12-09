using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutCreationException : PayoutException
    {
        private const string BitPayCode = "BITPAY-PAYOUT-CREATE";
        private const string BitPayMessage = "Failed to create payout.";
        protected string ApiCode;

        public PayoutCreationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public PayoutCreationException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }

        public String GetApiCode()
        {
            return ApiCode;
        }
    }
}