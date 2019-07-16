using System;

namespace BitPaySDK.Exceptions
{
    public class TokenNotFoundException : TokensCacheException
    {
        private const string BitPayCode = "BITPAY-TOKENS-NOTFOUND";
        private const string BitPayMessage = "There is no token for the specified key";

        public TokenNotFoundException(string key) : base(BitPayCode, BitPayMessage)
        {
            TokenKey = key;
        }

        public TokenNotFoundException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        public string TokenKey { get; } = "";
    }
}