using System;

namespace BitPaySDK.Exceptions
{
    public class TokensCacheWriteException : TokensCacheException
    {
        private const string BitPayCode = "BITPAY-TOKENS-WRITE";
        private const string BitPayMessage = "Error when trying to persist the tokens";

        public TokensCacheWriteException() : base(BitPayCode, BitPayMessage)
        {
        }

        public TokensCacheWriteException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}