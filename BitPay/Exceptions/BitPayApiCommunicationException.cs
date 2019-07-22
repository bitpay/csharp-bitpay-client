using System;

namespace BitPaySDK.Exceptions
{
    public class BitPayApiCommunicationException : BitPayException
    {
        private const string BitPayMessage = "Error when communicating with the BitPay API";
        private static readonly string _bitpayCode = "BITPAY-API";

        public BitPayApiCommunicationException() : base(BitPayMessage)
        {
            BitpayCode = _bitpayCode;
        }

        public BitPayApiCommunicationException(Exception ex) : base(BitPayMessage, ex)
        {
            BitpayCode = _bitpayCode;
        }

        public BitPayApiCommunicationException(string message) : base(_bitpayCode, message)
        {
        }

        public BitPayApiCommunicationException(string bitpayCode, string message) : base(bitpayCode, message)
        {
        }

        public BitPayApiCommunicationException(string bitpayCode, string message, Exception cause) : base(bitpayCode,
            message, cause)
        {
        }
    }
}