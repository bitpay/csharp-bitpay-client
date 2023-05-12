// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class RefundCreationException : RefundException
    {
        private new const string BitPayCode = "BITPAY-REFUND-CREATE";
        private const string BitPayMessage = "Failed to create refund";

        public RefundCreationException() : base(BitPayCode, BitPayMessage)
        {
        }
        
        public RefundCreationException(string message) : base(BitPayCode, message)
        {
        }

        public RefundCreationException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected RefundCreationException(SerializationInfo serializationInfo, StreamingContext streamingContext) 
            : base(serializationInfo, streamingContext)
        {
        }
    }
}