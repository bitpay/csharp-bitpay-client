// Copyright (c) 2019 BitPay.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace BitPay.Exceptions
{
    [Serializable]
    public class RefundCancellationException : RefundException
    {
        private new const string BitPayCode = "BITPAY-REFUND-CANCELLATION";
        private const string BitPayMessage = "Failed to cancel refund";

        public RefundCancellationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public RefundCancellationException(Exception ex, string? apiCode = "000000") 
            : base(BitPayCode, BitPayMessage, ex, apiCode)
        {
        }

        protected RefundCancellationException(SerializationInfo serializationInfo, StreamingContext streamingContext) 
            : base(serializationInfo, streamingContext)
        {
        }
    }
}