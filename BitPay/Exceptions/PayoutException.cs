using System;

namespace BitPaySDK.Exceptions
{
    public class PayoutException : BitPayException
    {
        private const string BitPayMessage = "An unexpected error occured while trying to manage the payout.";
        private readonly string _bitpayCode = "BITPAY-PAYOUT-GENERIC";

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

        public PayoutException(string bitpayCode, string message, Exception cause) : base(bitpayCode, message, cause)
        {
        }
    }
}