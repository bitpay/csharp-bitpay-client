using System;

namespace BitPay.Exceptions
{
    public class BitPayApiCommunicationException : BitPayException
    {
        private const string BitPayMessage = "Error when communicating with the BitPay API";
        private static readonly string _bitpayCode = "BITPAY-API";
        protected string ApiCode;

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

        public BitPayApiCommunicationException(string apiCode, string message) : base(apiCode, message, true)
        {
        }

        public BitPayApiCommunicationException(string bitpayCode, string message, Exception cause, string apiCode = "000000") : base(bitpayCode, message, cause, apiCode)
        {
        }

        public String GetApiCode()
        {
            return ApiCode;
        }
    }
}