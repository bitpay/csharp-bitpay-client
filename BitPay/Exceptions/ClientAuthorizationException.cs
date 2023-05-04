// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class ClientAuthorizationException : BitPayException
    {
        private new const string BitPayCode = "BITPAY-CLIENT-AUTH";
        private const string BitPayMessage = "Client authorization failed";

        public ClientAuthorizationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public ClientAuthorizationException(Exception ex, string? apiCode = "000000")
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        public ClientAuthorizationException(string bitPayCode, string message, Exception cause, string apiCode = "000000")
            : base(bitPayCode, message, cause, apiCode)
        {
        }

        protected ClientAuthorizationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}