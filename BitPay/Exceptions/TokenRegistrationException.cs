// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class TokenRegistrationException : TokensCacheException
    {
        private new const string BitPayCode = "BITPAY-POST-TOKEN";
        private const string BitPayMessage = "Token registration failed";

        public TokenRegistrationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public TokenRegistrationException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        protected TokenRegistrationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}