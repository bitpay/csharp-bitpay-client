using System;

namespace BitPay.Exceptions
{
    public class RatesException : BitPayException
    {
        private const string BitPayMessage = "An unexpected error occured while trying to manage the rates";
        private readonly string _bitpayCode = "BITPAY-RATES-GENERIC";

        public RatesException() : base(BitPayMessage)
        {
            BitpayCode = _bitpayCode;
        }

        public RatesException(Exception ex) : base(BitPayMessage, ex)
        {
            BitpayCode = _bitpayCode;
        }

        public RatesException(string bitpayCode, string message) : base(bitpayCode, message)
        {
        }

        public RatesException(string bitpayCode, string message, Exception cause, string apiCode = "000000") : base(bitpayCode, message, cause, apiCode)
        {
            ApiCode = apiCode;
        }
    }
}