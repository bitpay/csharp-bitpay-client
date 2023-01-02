using System;

namespace BitPay.Exceptions
{
    public class PayoutException : BitPayException
    {
        private const string BitPayMessage = "An unexpected error occured while trying to manage the payout.";
        private readonly string _bitpayCode = "BITPAY-PAYOUT-GENERIC";
        protected string ApiCode;

        public PayoutException() : base(BitPayMessage)
        {
            BitpayCode = _bitpayCode;
        }

        public PayoutException(Exception ex) : base(BitPayMessage, ex)
        {
            BitpayCode = _bitpayCode;
        }

        public PayoutException(string bitpayCode, string message) : base(bitpayCode, message)
        {
        }

        public PayoutException(string bitpayCode, string message, Exception cause, string apiCode = "000000") : base(bitpayCode, message, cause, apiCode)
        {
            ApiCode = apiCode;
        }

        public String GetApiCode()
        {
            return ApiCode;
        }

    }
}