// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class ConfigNotFoundException : BitPayException
    {
        private const string BitPayMessage = "An unexpected error occured while trying to retrieve the configuration";
        private new const string BitPayCode = "BITPAY-CONFIG-READ";

        public ConfigNotFoundException() : base(BitPayCode, BitPayMessage)
        {
        }

        public ConfigNotFoundException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        public ConfigNotFoundException(string bitPayCode, string message) : base(bitPayCode, message)
        {
        }

        public ConfigNotFoundException(string bitPayCode, string message, Exception cause)
            : base(bitPayCode, message, cause)
        {
        }

        protected ConfigNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}