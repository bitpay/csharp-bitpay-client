using System;

namespace BitPay.Exceptions
{
    public class RefundException : BitPayException
    {
        private const string BitPayMessage = "An unexpected error occured while trying to manage the refund";
        private readonly string _bitpayCode = "BITPAY-REFUND-GENERIC";

        public RefundException() : base(BitPayMessage)
        {
            BitpayCode = _bitpayCode;
        }

        public RefundException(Exception ex) : base(BitPayMessage, ex)
        {
            BitpayCode = _bitpayCode;
        }

        public RefundException(string bitpayCode, string message) : base(bitpayCode, message)
        {
        }

        public RefundException(string bitpayCode, string message, Exception cause, string apiCode = "000000") : base(bitpayCode, message, cause, apiCode)
        {
            ApiCode = apiCode;
        }
    }
}