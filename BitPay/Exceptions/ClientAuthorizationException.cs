using System;

namespace BitPaySDK.Exceptions
{
    public class ClientAuthorizationException : BitPayException
    {
        private const string BitPayCode = "BITPAY-CLIENT-AUTH";
        private const string BitPayMessage = "Client authorization failed";

        public ClientAuthorizationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public ClientAuthorizationException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}