using System;

namespace BitPaySDK.Exceptions
{
    public class TokensCacheLoadException : TokensCacheException
    {
        private const string BitPayCode = "BITPAY-TOKENS-LOAD";
        private const string BitPayMessage = "Error when trying to load the tokens";

        public TokensCacheLoadException() : base(BitPayCode, BitPayMessage)
        {
        }

        public TokensCacheLoadException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}