using System;

namespace BitPaySDK.Exceptions
{
    public class ConfigNotFoundException : BitPayException
    {
        private const string BitPayMessage = "An unexpected error occured while trying to retrieve the configuration";
        private readonly string _bitpayCode = "BITPAY-CONFIG-READ";

        public ConfigNotFoundException() : base(BitPayMessage)
        {
            BitpayCode = _bitpayCode;
        }

        public ConfigNotFoundException(Exception ex) : base(BitPayMessage, ex)
        {
            BitpayCode = _bitpayCode;
        }

        public ConfigNotFoundException(string bitpayCode, string message) : base(bitpayCode, message)
        {
        }

        public ConfigNotFoundException(string bitpayCode, string message, Exception cause) : base(bitpayCode, message,
            cause)
        {
        }
    }
}