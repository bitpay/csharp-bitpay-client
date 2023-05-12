// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class TokensCacheWriteException : TokensCacheException
    {
        private new const string BitPayCode = "BITPAY-TOKENS-WRITE";
        private const string BitPayMessage = "Error when trying to persist the tokens";

        public TokensCacheWriteException() : base(BitPayCode, BitPayMessage)
        {
        }

        public TokensCacheWriteException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        protected TokensCacheWriteException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}