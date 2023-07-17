// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class TokenNotFoundException : TokensCacheException
    {
        private new const string BitPayCode = "BITPAY-TOKENS-NOTFOUND";
        private const string BitPayMessage = "There is no token for the specified key";

        public TokenNotFoundException(string key) : base(BitPayCode, BitPayMessage)
        {
            TokenKey = key;
        }

        public TokenNotFoundException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        protected TokenNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        public string TokenKey { get; } = "";
    }
}