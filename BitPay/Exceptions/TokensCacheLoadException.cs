// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class TokensCacheLoadException : TokensCacheException
    {
        private new const string BitPayCode = "BITPAY-TOKENS-LOAD";
        private const string BitPayMessage = "Error when trying to load the tokens";

        public TokensCacheLoadException() : base(BitPayCode, BitPayMessage)
        {
        }

        public TokensCacheLoadException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        protected TokensCacheLoadException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}