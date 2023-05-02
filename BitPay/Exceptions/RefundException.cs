// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class RefundException : BitPayException
    {
        private const string BitPayMessage = "An unexpected error occured while trying to manage the refund";
        private new const string BitPayCode = "BITPAY-REFUND-GENERIC";

        public RefundException() : base(BitPayCode, BitPayMessage)
        {
        }

        public RefundException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }

        public RefundException(string bitPayCode, string message) : base(bitPayCode, message)
        {
        }

        public RefundException(string bitPayCode, string message, Exception cause, string? apiCode = "000000") 
            : base(bitPayCode, message, cause, apiCode)
        {
        }

        protected RefundException(SerializationInfo serializationInfo, StreamingContext streamingContext) 
            : base(serializationInfo, streamingContext)
        {
        }
    }
}