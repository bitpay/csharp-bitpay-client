using System;

namespace BitPaySDK.Exceptions
{
    public class TokenRegistrationException : TokensCacheException
    {
        private const string BitPayCode = "BITPAY-POST-TOKEN";
        private const string BitPayMessage = "Token registration failed";

        public TokenRegistrationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public TokenRegistrationException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}