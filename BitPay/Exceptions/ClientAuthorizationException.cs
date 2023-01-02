using System;

namespace BitPay.Exceptions
{
    public class ClientAuthorizationException : BitPayException
    {
        private const string BitPayCode = "BITPAY-CLIENT-AUTH";
        private const string BitPayMessage = "Client authorization failed";
        protected string ApiCode;

        public ClientAuthorizationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public ClientAuthorizationException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }

        public ClientAuthorizationException(string bitpayCode, string message, Exception cause, string apiCode = "000000") : base(bitpayCode, message, cause, apiCode)
        {
            ApiCode = apiCode;
        }

        public String GetApiCode()
        {
            return ApiCode;
        }
    }
}