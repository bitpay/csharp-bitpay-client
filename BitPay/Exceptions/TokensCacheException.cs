// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class TokensCacheException : BitPayException
    {
        private new const string BitPayCode = "BITPAY-TOKENSCACHE-GENERIC";
        private const string BitPayMessage = "An unexpected error occured while trying to manage the tokens cache";

        public TokensCacheException() : base(BitPayCode, BitPayMessage)
        {
        }

        public TokensCacheException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        public TokensCacheException(string bitPayCode, string message) : base(bitPayCode, message)
        {
        }

        public TokensCacheException(string bitPayCode, string message, Exception cause)
            : base(bitPayCode, message, cause)
        {
        }

        protected TokensCacheException(SerializationInfo serializationInfo, StreamingContext streamingContext) 
            : base(serializationInfo, streamingContext)
        {
        }
    }
}